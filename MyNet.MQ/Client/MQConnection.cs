using MyNet.Channel;
using MyNet.ClientConnection;

namespace MyNet.MQ.Client
{
    public class MQConnection : Connection
    {
        public MQConnection(string key, ChannelBase channel) : base(key, channel)
        {
        }
        /// <summary>
        /// 发布事件
        /// </summary>
        public bool Publish(MQMessage msg)
        {
            MQConnectionHandler hd = _channel.Pipeline.Tail.Prev.Handler as MQConnectionHandler;
            if (hd == null) return false;
            if (msg.TriggeredDate > 0)
            {
                if (string.IsNullOrEmpty(msg.TriggerId))
                {
                    msg.Content = string.Format("T[{0}]{1}", msg.TriggeredDate, msg.Content);
                }
                else
                {
                    msg.Content = string.Format("T[{0},{1}]{2}", msg.TriggeredDate, msg.TriggerId, msg.Content);
                }
            }
            return hd.Publish(_channel, msg);
        }
        public bool NoTrigger(string id)
        {
            MQConnectionHandler hd = _channel.Pipeline.Tail.Prev.Handler as MQConnectionHandler;
            if (hd == null) return false;
            MQMessage msg = new MQMessage();
            msg.Topic = string.Empty;
            msg.QosLevel = 2;
            msg.TriggeredDate = 0;
            msg.Content = id;
            return hd.Publish(_channel, msg);
        }
        /// <summary>
        /// 订阅事件
        /// </summary>
        public bool Subscribe(string queuename, byte level)
        {
            MQConnectionHandler hd = _channel.Pipeline.Tail.Prev.Handler as MQConnectionHandler;
            if (hd == null) return false;
            return hd.Subscribe(_channel, queuename, level);
        }
        public void Connect(string clientid)
        {
            MQConnectionHandler hd = _channel.Pipeline.Tail.Prev.Handler as MQConnectionHandler;
            if (hd == null)
            {
                _channel.Dispose();
                return;
            }
            hd.Connect(clientid, _channel);
        }
        /// <summary>
        /// 手动关闭连接
        /// </summary>
        public void Disconnect()
        {
            MQConnectionHandler hd = _channel.Pipeline.Tail.Prev.Handler as MQConnectionHandler;
            if (hd == null)
            {
                _channel.Dispose();
                return;
            }
            hd.Disconnect(_channel);
        }
    }
}
