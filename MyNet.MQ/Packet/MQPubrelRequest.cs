using MyNet.Common;

namespace MyNet.MQ.Packet
{
    /// <summary>
    /// pubrec的qos2级回应包
    /// </summary>
    public class MQPubrelRequest : MQAckRequest
    {
        private ushort _ack;
        public MQPubrelRequest(ushort ack)
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
            return (byte)(((byte)MQTTType.PUBREL << 4) | hdsign);
        }
    }
}
