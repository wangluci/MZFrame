using MyNet.Common;

namespace MyNet.MQ.Packet
{
    /// <summary>
    /// mqpublishrequest的qos2级回应包
    /// </summary>
    public class MQPubrecRequest : MQAckRequest
    {
        private ushort _ack;
        public MQPubrecRequest(ushort ack)
        {
            _ack = ack;
        }
        public override byte[] GenerateContent()
        {
            return Converter.ToBytes(_ack);
        }

        public override byte GenerateHeader()
        {
            byte hdsign = 0;
            if (_isrepeat)
            {
                hdsign |= 0x08;
            }
            return (byte)(((byte)MQTTType.PUBREC << 4) | hdsign);
        }
    }
}
