using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Common;
using MyNet.Handlers;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using MyNet.MQ.Packet;
using MyNet.MQ.Parse;
using MyNet.MQ.Session;
using System;
using System.Collections.Generic;

namespace MyNet.MQ
{
    /// <summary>
    /// MQ协议处理,协议格式：类型一字节，内容长度两字节，内容
    /// 0为ping,1为pong
    /// </summary>
    internal class MQHandler : IdleStateHandler
    {
        private HostConfig _config;
        public HostConfig Config { get { return _config; } }
        public MQSession Session { get; set; }

        private ITriggerRunnable _connTimeout;
        public bool IsConnected { get; set; }
        public void RestartIdleTime(IContext ctx, int seconds)
        {
            if (_connTimeout != null)
            {
                if (_connTimeout.Cancel())
                {
                    ResetReaderIdleTime((int)(seconds * 1.5));
                    RestartIdleTimeOut(ctx);
                    //监控是否保持连接
                    ctx.Channel.AddListener(CHANNEL_READ_IDLE, (EventArgs e) =>
                    {
                        ctx.Channel.Dispose();
                    });
                }
            }
        }
        public MQHandler(HostConfig config) : base(0, 0, 0)
        {
            IsConnected = false;
            _config = config;
        }


        /// <summary>
        /// 处理接收包
        /// </summary>
        /// <param name="context"></param>
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
            if (mqtype != MQTTType.CONNECT && !IsConnected)
            {
                context.Channel.Dispose();
                return;
            }
            IMQParse parse;
            switch (mqtype)
            {
                case MQTTType.CONNECT:
                    parse = new MQConnParse();
                    break;
                case MQTTType.PUBLISH:
                    parse = new MQPublishParse();
                    break;
                case MQTTType.PUBREL:
                    parse = new MQPubRelParse();
                    break;
                case MQTTType.SUBSCRIBE:
                    parse = new MQSubscribeParse();
                    break;
                case MQTTType.UNSUBSCRIBE:
                    parse = new MQUnSubscribeParse();
                    break;
                case MQTTType.PINGREQ:
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
                default:
                    return;
            }
            //解释
            if (parse.ServerParse(context, content, this, dup, qos, retain))
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
            if (context.Channel is ServerChannel)
            {
                Common.AgentLogger.Instance.Info("站点开启");
            }
            else
            {
                //5秒未连接则关闭
                _connTimeout = context.Loop.Schedule(c =>
                  {
                      c.Dispose();
                  }, context.Channel, 5000);
            }
            context.FireNextActive();
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

        public override void ChannelInactive(IContext context)
        {
            if (context.Channel is ServerChannel)
            {
                AgentLogger.Instance.Info("站点关闭");
            }
            else
            {
                if (Session != null)
                {
                    MQSessionManager.Instance().CloseSession(Session.SessionId, Session.ClearAtClose);
                    Session = null;
                }
            }
            context.FireNextInactive();
        }
        public void ResendHandler(EventArgs e)
        {
            if (Session != null)
            {
                Session.ResendNotAcked();
            }
        }
        public override void HandlerInstalled(IContext context)
        {

        }

        public override void HandlerUninstalled(IContext context)
        {
        }


    }
}
