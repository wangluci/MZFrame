
namespace MyNet.MQ.Packet
{
    public class MQPongResponse : MQPacket
    {
        public MQPongResponse()
        {
        }
        public override byte[] GenerateContent()
        {
            return new byte[0];
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.PINGRESP << 4;
        }
    }
}
