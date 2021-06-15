using MyNet.Buffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNet.MQ.Packet
{
    public class MQUnsubscribeRequest : MQPacket
    {
        private ushort _ack;
        private List<string> _topics;
        public MQUnsubscribeRequest(ushort ack,List<string> topics)
        {
            _ack = ack;
            _topics = topics;
        }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();
            stream.WriteShort((short)_ack);
            foreach (string t in _topics)
            {
                stream.WriteShort((short)t.Length);
                stream.WriteBytes(Encoding.UTF8.GetBytes(t));
            }
            return stream.ToArray();
        }

        public override byte GenerateHeader()
        {
            return ((byte)MQTTType.UNSUBSCRIBE << 4) | 0x02;
        }
    }
}
