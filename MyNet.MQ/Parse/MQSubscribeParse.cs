
using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using System.Text;
using System.Collections.Generic;
using MyNet.MQ.Packet;
using MyNet.MQ.Session;

namespace MyNet.MQ.Parse
{
    internal class MQSubscribeParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            handler.FinishSubscribe(context);
            return true;
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                List<MQFilter> filters = new List<MQFilter>();
                do
                {
                    ushort topiclen = (ushort)data.ReadShort();
                    string topicfilter = Encoding.UTF8.GetString(data.ReadBytes(topiclen));
                    byte qoslevel = data.ReadByte();
                    filters.Add(new MQFilter(topicfilter, qoslevel));
                }
                while (data.Length > data.ReaderIndex);

                List<SubsCode> codes = MQSessionManager.Instance().Subscribe(handler.Session.SessionId, filters);
                context.Channel.SendAsync(new MQSubscribeResponse(ack, codes));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
