using MyNet.Common;
namespace MyNet.MQ.Packet
{
    /// <summary>
    /// qos1级回应包
    /// </summary>
    public class MQPublishResponse : MQPacket
    {
        private ushort _ack;
        public MQPublishResponse(ushort ack)
        {
            _ack = ack;
        }
        public override byte[] GenerateContent()
        {
            return Converter.ToBytes(_ack);
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.PUBACK << 4;
        }
    }
}
