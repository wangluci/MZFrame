using MyNet.Common;
using MyNet.Common.TimeWheel;
using MyNet.Handlers;
using MyNet.MQ.Customer;
using MyNet.MQ.Packet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyNet.MQ.Session
{
    public class MQSessionManager : BaseDisposable
    {
        private HostConfig _config;
        private volatile static MQSessionManager mSingleton;
        private static object lockHelper = new object();
        private ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();
        private Dictionary<string, MQSession> SessionPools = new Dictionary<string, MQSession>();
        private MQTree _tree = new MQTree();
        private MQThreadQueue[] _queueThreads;
        private ConcurrentDictionary<string, IWheelTimeout> _wheels;
        private MQSessionManager()
        {
            _wheels = new ConcurrentDictionary<string, IWheelTimeout>();
        }
        public static MQSessionManager Instance()
        {
            if (mSingleton == null)
            {
                lock (lockHelper)
                {
                    if (mSingleton == null)
                    {
                        mSingleton = new MQSessionManager();
                    }
                }
            }
            return mSingleton;
        }

        public void Init(HostConfig config)
        {
            _config = config;
            //重置队列，将持久化数据压入队列
            _queueThreads = new MQThreadQueue[config.DelayQueueCount];
            for (int i = 0; i < config.DelayQueueCount; i++)
            {
                MQMessage[] msgs = _config.Serial.LoadInitData(i, config.DelayQueueCount);
                _queueThreads[i] = new MQThreadQueue();
                foreach (MQMessage msg in msgs)
                {
                    IWheelTimeout whe = _queueThreads[i].EnqueueMsg(msg);
                    if (!string.IsNullOrEmpty(msg.TriggerId))
                    {
                        _wheels.AddOrUpdate(msg.TriggerId, whe, (oldkey, oldvalue) => whe);
                    }
                }
            }
        }
        public MQSession NewSession(string id, IContext context)
        {
            try
            {
                _lockSlim.EnterWriteLock();
                MQSession session;
                if (!SessionPools.TryGetValue(id, out session))
                {
                    session = new MQSession(id);
                    SessionPools.Add(id, session);
                }
                else
                {
                    if (session.Context != null)
                    {
                        session.Context.Channel.Dispose();
                    }
                }

                session.Context = context;
                return session;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }

        }
        public void CloseSession(string key, bool clear)
        {
            if (clear)
            {
                try
                {
                    _lockSlim.EnterWriteLock();
                    //清除订阅信息
                    MQSession session;
                    if (SessionPools.TryGetValue(key, out session))
                    {
                        //将发送失败消息放入延时队列
                        foreach (MQMessage msg in session.Publics.Values)
                        {
                            JoinThreadQueue(msg, false);
                        }
                        //取消订阅
                        foreach (MQConsumer c in session.Consumers.Values)
                        {
                            _tree.RemoveFromTree(c.TopicFilter, key);
                        }
                        SessionPools.Remove(key);
                    }

                }
                finally
                {
                    _lockSlim.ExitWriteLock();
                }
            }

        }

        private void JoinThreadQueueAndSave(MQMessage msg)
        {
            if (string.IsNullOrEmpty(msg.MessageId))
            {
                msg.MessageId = _config.NextId();
                _config.Serial.SerialMessage(msg);
                JoinThreadQueue(msg, true);
            }
            else
            {
                JoinThreadQueue(msg, false);
            }
        }
        public void JoinThreadQueue(MQMessage msg, bool first)
        {
            if (!first)
            {
                msg.TriggeredDate = Converter.Cast<long>(DateTime.Now.AddSeconds(10));
            }
            //加入延时队列
            int queueindex = (int)((uint)msg.MessageId.GetHashCode() % (uint)_config.DelayQueueCount);
            IWheelTimeout whe = _queueThreads[queueindex].EnqueueMsg(msg);
            if (!string.IsNullOrEmpty(msg.TriggerId))
            {
                _wheels.AddOrUpdate(msg.TriggerId, whe, (oldkey, oldvalue) => whe);
            }
        }
 
        public void NoTrigger(string id)
        {
            IWheelTimeout whe;
            if(_wheels.TryRemove(id,out whe))
            {
                whe.Cancel();
            }
        }
        /// <summary>
        /// 路由消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>是否路由成功</returns>
        public void RouteMessage(MQMessage msg)
        {
            try
            {
                _lockSlim.EnterReadLock();
                long nowtimes = Converter.Cast<long>(DateTime.Now);
                if (msg.TriggeredDate <= nowtimes)
                {
                    //直接发布
                    MQConsumer[] customers = _tree.Match(msg.Topic);
                    if (customers.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(msg.ClientId))
                        {
                            MQConsumer c = null;
                            foreach (MQConsumer ic in customers)
                            {
                                if (ic.SessionId.Equals(msg.ClientId))
                                {
                                    c = ic;
                                    break;
                                }
                            }
                            //如果消息已被指定消费者，则重新发给消费者
                            if (c != null)
                            {
                                MQSession session;
                                if (SessionPools.TryGetValue(c.SessionId, out session))
                                {
                                    session.ConsumeMessage(msg, Math.Min(msg.QosLevel, c.Qos));
                                }
                            }
                            else
                            {
                                JoinThreadQueue(msg, false);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(msg.MessageId))
                            {
                                _config.Serial.SuccessMessage(msg.MessageId);
                            }
                            foreach (MQConsumer c in customers)
                            {
                                //将消息复制
                                MQMessage newmsg = msg.Clone();
                                newmsg.ClientId = c.SessionId;
                                newmsg.MessageId = _config.NextId();
                                MQSession session;
                                if (SessionPools.TryGetValue(c.SessionId, out session))
                                {
                                    byte level = Math.Min(newmsg.QosLevel, c.Qos);
                                    if (level != 0)
                                    {
                                        _config.Serial.SerialMessage(newmsg);
                                    }
                                    session.ConsumeMessage(newmsg, level);
                                }
                            }
                        }
                    }
                    else
                    {
                        JoinThreadQueueAndSave(msg);
                    }
                }
                else
                {
                    JoinThreadQueueAndSave(msg);
                }
            }
            catch { }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }


        public List<SubsCode> Subscribe(string sessionid, List<MQFilter> filters)
        {
            try
            {
                _lockSlim.EnterWriteLock();
                List<SubsCode> rts = new List<SubsCode>();
                MQSession session;
                if (SessionPools.TryGetValue(sessionid, out session))
                {
                    foreach (MQFilter f in filters)
                    {
                        MQConsumer c = new MQConsumer(sessionid, f.TopicFilter, f.QosLevel);
                        _tree.AddToTree(c);
                        session.Consumers[c.TopicFilter.ToString()] = c;
                        rts.Add((SubsCode)f.QosLevel);
                    }
                }
                return rts;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
        public void UnSubscribe(string sessionid, List<string> filternames)
        {
            try
            {
                _lockSlim.EnterWriteLock();
                MQSession session;
                if (SessionPools.TryGetValue(sessionid, out session))
                {
                    foreach (string s in filternames)
                    {
                        session.Consumers.Remove(s);
                        _tree.RemoveFromTree(s, sessionid);
                    }
                }
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }

        }

        protected override void OnUnManDisposed()
        {
            if (_queueThreads != null)
            {
                foreach (MQThreadQueue mq in _queueThreads)
                {
                    if (mq != null)
                    {
                        mq.Close();
                    }
                }
            }
            SessionPools.Clear();
        }
    }
}
