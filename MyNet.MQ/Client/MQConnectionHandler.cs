using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Common;
using MyNet.Handlers;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using MyNet.MQ.Packet;
using MyNet.MQ.Parse;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MyNet.MQ.Client
{
    public class MQConnectionHandler : IdleStateHandler
    {
        private Queue<ITriggerRunnable> _timeout = new Queue<ITriggerRunnable>();
        private bool _reqrt;
        private AutoResetEvent _autoEvents;
        private ITriggerRunnable _subscribeTimeout;
        private ITriggerRunnable _publishTimeout;
        private ITriggerRunnable _connTimeout;
        private ClientConfig _config;
        private ushort _curack;
        public ClientConfig Config { get { return _config; } }
        private Dictionary<ushort, MQMessage> _recvs;
        public Dictionary<ushort, MQMessage> Recvs { get { return _recvs; } }
        private bool _isConnected;
        public MQConnectionHandler(ClientConfig config) : base(config.PingPongTime, 0, 0)
        {
            _isConnected = false;
            _recvs = new Dictionary<ushort, MQMessage>();
            _curack = 0;
            _reqrt = false;
            _autoEvents = new AutoResetEvent(false);
            _config = config;
        }
        private ushort NextAck()
        {
            _curack += 1;
            if (_curack >= ushort.MaxValue)
            {
                _curack = 0;
            }
            return _curack;
        }
        public void FinishNotice(IContext ctx, MQMessage msg)
        {
            if (_config.Listener != null)
            {
                Thread newThread = new Thread(() =>
                {
                    if (_config.Listener != null)
                    {
                        _config.Listener.ConsumeHandle(msg);
                    }
                });
                newThread.Start();
            }
        }
        public void FinishPublic(IContext ctx)
        {
            if (_publishTimeout != null)
            {
                if (_publishTimeout.Cancel())
                {
                    _publishTimeout = null;
                    _reqrt = true;
                }
            }
            _autoEvents.Set();
        }
        public void FinishSubscribe(IContext ctx)
        {
            if (_subscribeTimeout != null)
            {
                if (_subscribeTimeout.Cancel())
                {
                    _subscribeTimeout = null;
                    _reqrt = true;
                }
            }
            _autoEvents.Set();
        }
        public void FinishConn(IContext ctx, bool success)
        {
            if (_connTimeout != null)
            {
                if (_connTimeout.Cancel())
                {
                    _connTimeout = null;
                    _isConnected = success;
                    ctx.Channel.AddListener(CHANNEL_READ_IDLE, (EventArgs e) =>
                    {
                        MQPingRequest ping = new MQPingRequest();
                        ctx.Channel.SendAsync(ping);
                        ITriggerRunnable run = ctx.Loop.Schedule(c =>
                        {
                            c.Dispose();
                        }, ctx.Channel, 2000);
                        _timeout.Enqueue(run);
                    });
                }
            }
            _autoEvents.Set();
        }
        public void ClearTimeout()
        {
            if (_timeout.Count > 0)
            {
                _timeout.Dequeue().Cancel();
            }
        }
        public void Connect(string clientid, ChannelBase channel)
        {
            if (!_isConnected)
            {
                if (channel.Loop.InCurrentThread()) return;
                _connTimeout = channel.Loop.Schedule(() =>
                {
                    _connTimeout = null;
                    channel.Dispose();
                    _autoEvents.Set();
                }, 5000);
    
                MQConnRequest req = new MQConnRequest(_config.Account, _config.Password, _config.PingPongTime, clientid, _config.ClearSession);
                req.ErrHandler((IContext context, WritePacket packet) =>
                {
                    if (_connTimeout.Cancel())
                    {
                        _connTimeout = null;
                    }
                    _autoEvents.Set();
                });
                channel.Pipeline.FireChannelWrite(req);
                _autoEvents.WaitOne();
            }
        }
        public void Disconnect(ChannelBase channel)
        {
            channel.SendAsync(new MQDisconnect().Handler(WritePacket.Close));
        }
        public bool Publish(ChannelBase channel, MQMessage msg)
        {
            if (!_isConnected || channel.Loop.InCurrentThread())
            {
                _config.Serial.SerialErr(msg);
                return false;
            }
            _reqrt = false;
            _publishTimeout = channel.Loop.Schedule(c =>
            {
                _publishTimeout = null;
                c.Dispose();
                _autoEvents.Set();
            }, channel, 5000);

            MQPublishRequest request = new MQPublishRequest(msg, NextAck());
            request.ErrHandler((IContext context, WritePacket packet) =>
            {
                if (_publishTimeout.Cancel())
                {
                    _publishTimeout = null;
                }
                _autoEvents.Set();
            });
            channel.Pipeline.FireChannelWrite(request);
            _autoEvents.WaitOne();
            if (!_reqrt)
            {
                _config.Serial.SerialErr(msg);
            }
            return _reqrt;
        }

        public bool Subscribe(ChannelBase channel, string queuename, byte level)
        {
            if (channel.Loop.InCurrentThread()) return false;
            if (!_isConnected) return false;
            _reqrt = false;
            _subscribeTimeout = channel.Loop.Schedule(c =>
           {
               _subscribeTimeout = null;
               c.Dispose();
               _autoEvents.Set();
           }, channel, 10000);
            List<MQFilter> filters = new List<MQFilter>();
            filters.Add(new MQFilter(queuename, level));
            MQSubscribeRequest req = new MQSubscribeRequest(NextAck(), filters);
            req.ErrHandler((IContext context, WritePacket packet) =>
            {
                if (_subscribeTimeout.Cancel())
                {
                    _subscribeTimeout = null;
                }
                _autoEvents.Set();
            });
            channel.Pipeline.FireChannelWrite(req);
            _autoEvents.WaitOne();
            return _reqrt;
        }
        /// <summary>
        /// 处理接收包
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private void OnPacket(IContext context, IByteStream stream)
        {
            ChannelBase channel = context.Channel;
            stream.SetReaderIndex(0);
            ByteBits fixTypeSign = stream.ReadByte();
            MQTTType mqtype = (MQTTType)(byte)fixTypeSign.Take(4);
            byte mqsign = (byte)(fixTypeSign & 0xF);
            bool dup = false;
            if ((mqsign & 0x08) != 0)
            {
                dup = true;
            }
            byte qos = (byte)((mqsign & 0x06) >> 1);
            bool retain = false;
            if ((mqsign & 0x01) != 0)
            {
                retain = true;
            }

            int tlen = 0;
            byte tmp;
            int mul = 1;
            int maxlen = 2097152;
            do
            {
                tmp = stream.ReadByte();
                tlen += (tmp & 127) * mul;
                mul *= 128;
                if (mul > maxlen) return;
            } while ((tmp & 128) != 0);

            if (stream.Length - stream.ReaderIndex < tlen)
            {
                //数据未完整
                channel.MergeRead();
                return;
            }

            byte[] data = stream.ReadBytes(tlen);
            IByteStream content = PoolBufferAllocator.Default.AllocStream();
            content.WriteBytes(data);
            IMQParse parse;
            switch (mqtype)
            {
                case MQTTType.CONNACK:
                    parse = new MQConnParse();
                    break;
                case MQTTType.PINGRESP:
                    parse = new MQPingParse();
                    break;
                case MQTTType.PUBACK:
                    parse = new MQPubackParse();
                    break;
                case MQTTType.PUBREC:
                    parse = new MQPubrecParse();
                    break;
                case MQTTType.PUBCOMP:
                    parse = new MQPubcompParse();
                    break;
                case MQTTType.SUBACK:
                    parse = new MQSubscribeParse();
                    break;
                case MQTTType.UNSUBACK:
                    parse = new MQUnSubscribeParse();
                    break;
                case MQTTType.PUBLISH:
                    parse = new MQPublishParse();
                    break;
                case MQTTType.PUBREL:
                    parse = new MQPubRelParse();
                    break;
                default:
                    return;
            }
            if (parse.ClientParse(context, content, this, dup, qos, retain))
            {
                context.Channel.FinishRead();
            }
            else
            {
                context.Channel.Dispose();
                return;
            }
        }
        public override void ChildActive(IContext context)
        {
            context.FireNextActive();
        }
        public override void ChannelInactive(IContext context)
        {
            context.FireNextInactive();
        }
        public override void ChildRead(IContext context, object msg)
        {
            IByteStream stream = msg as IByteStream;
            if (stream != null)
            {
                OnPacket(context, stream);
            }
        }

        public override void ChannelWrite(IContext context, object msg)
        {
            MQPacket response = msg as MQPacket;
            if (response != null)
            {
                response.GenerateWritePacket();
            }
            context.FirePreWrite(msg);
        }

        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }

    }
}
