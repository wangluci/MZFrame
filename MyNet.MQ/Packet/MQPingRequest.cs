
namespace MyNet.MQ.Packet
{
    public class MQPingRequest : MQPacket
    {
        public MQPingRequest()
        {
        }
        public override byte[] GenerateContent()
        {
            return new byte[0];
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.PINGREQ << 4;
        }
    }
}
