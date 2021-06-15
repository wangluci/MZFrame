using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Middleware.Http.WebSocket;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyNet.SocketIO
{
    public class WebsocketState : TransportState
    {
        public WebsocketState(ConcurrentQueue<SocketIOPacket> queue) : base(queue)
        {
        }

        public override void Send(string sessionid)
        {
            while (true)
            {
                SocketIOPacket packet;
                if (!_queue.TryDequeue(out packet))
                {
                    break;
                }
                IByteStream stream = PoolBufferAllocator.Default.AllocStream();
                EncodePacket(stream, true, packet);
                byte[] array = stream.ToArray();
                stream.Dispose();
                FrameResponse response = new FrameResponse(FrameCodes.Text, array);
                if (packet.GetSubType() == SubPacketType.DISCONNECT)
                {
                    response.SuccessHandler(WritePacket.Close);
                }
                _context.Channel.SendAsync(response);
            }
        }
    }
}
