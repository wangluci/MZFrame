using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using MyNet.Loop.Scheduler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace MyNet.SocketIO
{
    public abstract class TransportState
    {
        protected ConcurrentQueue<SocketIOPacket> _queue;
        protected IContext _context;
        public TransportState(ConcurrentQueue<SocketIOPacket> queue)
        {
            _queue = queue;
        }
        public ITriggerRunnable Schedule(Action action, int interval)
        {
            return _context.Loop.Schedule(action, interval);
        }
        public virtual void UpdateContext(IContext context)
        {
            Interlocked.Exchange(ref _context, context);
        }
        protected void WriteCharPacketType(IByteStream stream, byte t)
        {
            stream.WriteBytes(Encoding.UTF8.GetBytes(t.ToString()));
        }

        /// <summary>
        /// 编码发送包
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buff"></param>
        /// <param name="binary">是否只有一个包，一次发多个包时应该为false</param>
        /// <param name="packet"></param>
        protected void EncodePacket(IByteStream buff, bool binary, SocketIOPacket packet)
        {
            IByteStream stream = buff;
            if (!binary)
            {
                stream = PoolBufferAllocator.Default.AllocStream();
            }
            PacketType t = packet.GetResponseType();

            WriteCharPacketType(stream, (byte)t);
            try
            {
                switch (t)
                {
                    case PacketType.PONG:
                        string pongstr = packet.GetData().ToString();
                        if (!string.IsNullOrEmpty(pongstr))
                        {
                            stream.WriteBytes(Encoding.UTF8.GetBytes(pongstr));
                        }
                        break;
                    case PacketType.OPEN:
                        stream.WriteBytes(Encoding.UTF8.GetBytes(MyAccess.Json.Json.Encode(packet.GetData(), true)));
                        break;
                    case PacketType.MESSAGE:
                        IByteStream encBuf = null;
                        if (packet.GetSubType() == SubPacketType.ERROR)
                        {
                            encBuf = PoolBufferAllocator.Default.AllocStream();
                            encBuf.WriteBytes(Encoding.UTF8.GetBytes(MyAccess.Json.Json.Encode(packet.GetData(), true)));
                        }
                        else if (packet.GetSubType() == SubPacketType.EVENT || packet.GetSubType() == SubPacketType.ACK)
                        {
                            List<object> output = new List<object>();
                            encBuf = PoolBufferAllocator.Default.AllocStream();
                            if (packet.GetSubType() == SubPacketType.EVENT)
                            {
                                output.Add(packet.GetName());
                            }
                            object obj = packet.GetData();
                            if (obj != null)
                            {
                                output.Add(obj);
                            }
                            encBuf.WriteBytes(Encoding.UTF8.GetBytes(MyAccess.Json.Json.Encode(output, true)));
                        }

                        WriteCharPacketType(stream, (byte)packet.GetSubType());


                        if (packet.GetSubType() == SubPacketType.CONNECT)
                        {
                            string nsp = packet.GetNsp();
                            if (!string.IsNullOrEmpty(nsp))
                            {
                                stream.WriteBytes(Encoding.UTF8.GetBytes(nsp));
                            }
                        }
                        else
                        {
                            string nsp = packet.GetNsp();
                            if (!string.IsNullOrEmpty(nsp) && !SocketIO.DEFAULT_NAME.Equals(nsp))
                            {
                                stream.WriteBytes(Encoding.UTF8.GetBytes(nsp));
                                stream.WriteBytes(Encoding.UTF8.GetBytes(","));
                            }
                        }

                        if (packet.GetAckId() >= 0)
                        {
                            stream.WriteBytes(Encoding.UTF8.GetBytes(packet.GetAckId().ToString()));
                        }

                        if (encBuf != null)
                        {
                            if (encBuf.Length > 0)
                            {
                                stream.WriteBytes(encBuf.ToArray());
                            }
                            encBuf.Dispose();
                        }
                        break;
                }
            }
            finally
            {
                if (!binary)
                {
                    buff.WriteBytes(Encoding.UTF8.GetBytes(stream.Length.ToString()));
                    buff.WriteBytes(Encoding.UTF8.GetBytes(":"));
                    buff.WriteBytes(stream.ToArray());
                    stream.Dispose();
                }
            }

        }
        public abstract void Send(string sessionid);
    }
}
