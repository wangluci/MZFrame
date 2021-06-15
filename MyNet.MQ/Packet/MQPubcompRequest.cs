
using MyNet.Buffer;
using MyNet.Common;
using System;

namespace MyNet.MQ.Packet
{
    /// <summary>
    /// pubcom的qos2级回应包
    /// </summary>
    public class MQPubcompRequest : MQPacket
    {
        private ushort _ack;
        public MQPubcompRequest(ushort ack)
        {
            _ack = ack;
        }
        public override byte[] GenerateContent()
        {
            return Converter.ToBytes(_ack);
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.PUBCOMP << 4;
        }
    }
}
