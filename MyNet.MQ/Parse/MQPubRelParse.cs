using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using MyNet.MQ.Packet;
using MyNet.MQ.Session;

namespace MyNet.MQ.Parse
{
    internal class MQPubRelParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                MQMessage msg;
                if (!handler.Recvs.TryGetValue(ack, out msg))
                {
                    context.Channel.SendAsync(new MQPubcompRequest(ack));
                    return true;
                }
                handler.Recvs.Remove(ack);
                handler.FinishNotice(context, msg);
                context.Channel.SendAsync(new MQPubcompRequest(ack));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                handler.Session.RemoveSendReq(ack);
                MQMessage msg;
                if(!handler.Session.Qos2Recvs.TryGetValue(ack,out msg))
                {
                    context.Channel.SendAsync(new MQPubcompRequest(ack));
                    return true;
                }
                handler.Session.Qos2Recvs.Remove(ack);
                if (string.IsNullOrEmpty(msg.Topic))
                {
                    MQSessionManager.Instance().NoTrigger(msg.Content);
                }
                else
                {
                    MQSessionManager.Instance().RouteMessage(msg);
                }
                context.Channel.SendAsync(new MQPubcompRequest(ack));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
