
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;

namespace MyNet.MQ.Parse
{
    internal interface IMQParse
    {
        bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain);
        bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain);
    }
}
