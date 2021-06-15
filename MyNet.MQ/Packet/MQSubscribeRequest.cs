using MyNet.Buffer;
using System.Collections.Generic;
using System.Text;

namespace MyNet.MQ.Packet
{
    public class MQSubscribeRequest : MQPacket
    {
        private ushort _ack;
        private List<MQFilter> _filters;
        public MQSubscribeRequest(ushort ack, List<MQFilter> filters)
        {
            _ack = ack;
            _filters = filters;
        }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();
            stream.WriteShort((short)_ack);
            foreach(MQFilter f in _filters)
            {
                stream.WriteShort((short)f.TopicFilter.Length);
                stream.WriteBytes(Encoding.UTF8.GetBytes(f.TopicFilter));
                stream.WriteByte(f.QosLevel);
            }
            return stream.ToArray();
        }

        public override byte GenerateHeader()
        {
            return ((byte)MQTTType.SUBSCRIBE << 4) | 0x02;
        }
    }
}
