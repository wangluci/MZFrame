using MyNet.Buffer;
using System;
using System.Collections.Generic;

namespace MyNet.MQ.Packet
{
    public class MQSubscribeResponse : MQPacket
    {
        private ushort _ack;
        private List<SubsCode> _codes;
        public MQSubscribeResponse(ushort ack, List<SubsCode> codes)
        {
            _ack = ack;
            _codes = codes;
        }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();
            stream.WriteShort((short)_ack);
            foreach(SubsCode s in _codes)
            {
                stream.WriteByte((byte)s);
            }
            return stream.ToArray();
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.SUBACK << 4;
        }
    }
}
