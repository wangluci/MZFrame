using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using MyNet.MQ.Packet;

namespace MyNet.MQ.Parse
{
    internal class MQPubrecParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                context.Channel.SendAsync(new MQPubrelRequest(ack));
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
                handler.Session.SendNeedAck(ack, new MQPubrelRequest(ack));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
