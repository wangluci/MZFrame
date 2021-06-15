using MyNet.Common;
namespace MyNet.MQ.Packet
{
    public class MQUnsubscribeResponse : MQPacket
    {
        private ushort _ack;
        public MQUnsubscribeResponse(ushort ack)
        {
            _ack = ack;
        }
        public override byte[] GenerateContent()
        {
            return Converter.ToBytes(_ack);
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.UNSUBACK << 4;
        }
    }
}
