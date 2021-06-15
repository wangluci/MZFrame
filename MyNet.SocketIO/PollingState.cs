using MyNet.Middleware.Http;
using System.Text;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.Channel;
using System.Collections.Concurrent;

namespace MyNet.SocketIO
{
    public class PollingState : TransportState
    {
        public PollingState(ConcurrentQueue<SocketIOPacket> queue) : base(queue)
        {
        }
        public override void UpdateContext(IContext context)
        {
            base.UpdateContext(context);
            object targetobj;
            context.Channel.Propertys.TryRemove("ResponsedOnce", out targetobj);
        }
   
        public override void Send(string sessionid)
        {
            if (_queue.IsEmpty || _context == null) return;
            ChannelBase channel = _context.Channel;
            bool isresponsed = channel.Propertys.TryAdd("ResponsedOnce", "Ok");
            if (!isresponsed) return;
            //http需要发送一次清除context
            bool b64 = channel.Propertys.TryGet("b64", false);
            string origin = channel.Propertys.TryGet("origin", "");
            bool sendandclose = false;
            HttpResponse response = SocketIO.CreateResponse(sessionid, origin);
            int limit = 0;
            if (b64)
            {

                while (true)
                {
                    if (limit == 50)
                    {
                        break;
                    }
                    SocketIOPacket packet;
                    if (!_queue.TryDequeue(out packet))
                    {
                        break;
                    }
                    if (packet.GetSubType() == SubPacketType.DISCONNECT)
                    {
                        sendandclose = true;
                    }

                    IByteStream stream = PoolBufferAllocator.Default.AllocStream();
                    EncodePacket(stream, true, packet);
                    response.Write(Encoding.UTF8.GetBytes(stream.Length.ToString()));
                    response.Write(Encoding.UTF8.GetBytes(":"));
                    response.Write(stream.ToArray());
                    stream.Dispose();
                    limit++;
                }
                response.ContentType = "text/plain";
            }
            else
            {
                while (true)
                {
                    if (limit == 50)
                    {
                        break;
                    }

                    SocketIOPacket packet;
                    if (!_queue.TryDequeue(out packet))
                    {
                        break;
                    }
                    if (packet.GetSubType() == SubPacketType.DISCONNECT)
                    {
                        sendandclose = true;
                    }
                    EncodePacket(response.Content, false, packet);
                    limit++;
                }
                response.ContentType = "application/octet-stream";
            }
            if (sendandclose)
            {
                response.SuccessHandler(Channel.WritePacket.Close);
            }
            channel.SendAsync(response);
        }
    }
}
