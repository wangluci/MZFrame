using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;

namespace MyNet.MQ.Parse
{
    internal class MQPubcompParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            handler.FinishPublic(context);
            return true;
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                handler.Session.RemoveSendReq(ack);
                MQMessage msg;
                if (!handler.Session.Publics.TryGetValue(ack, out msg))
                {
                    return true;
                }
                handler.Session.Publics.Remove(ack);
                handler.Config.Serial.SuccessMessage(msg.MessageId);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
