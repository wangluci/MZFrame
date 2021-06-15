using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using MyNet.MQ.Packet;

namespace MyNet.MQ.Parse
{
    internal class MQPingParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            handler.ClearTimeout();
            return true;
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            context.Channel.SendAsync(new MQPongResponse());
            return true;
        }
    }
}
