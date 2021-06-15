using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using System.Collections.Generic;
using System.Text;
using MyNet.MQ.Packet;
using MyNet.MQ.Session;

namespace MyNet.MQ.Parse
{
    internal class MQUnSubscribeParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            throw new NotImplementedException();
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = (ushort)data.ReadShort();
                List<string> filternames = new List<string>();
                do
                {
                    ushort topiclen = (ushort)data.ReadShort();
                    string topicfilter = Encoding.UTF8.GetString(data.ReadBytes(topiclen));
                    filternames.Add(topicfilter);
                }
                while (data.Length > data.ReaderIndex);

                MQSessionManager.Instance().UnSubscribe(handler.Session.SessionId, filternames);
                context.Channel.SendAsync(new MQUnsubscribeResponse(ack));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
