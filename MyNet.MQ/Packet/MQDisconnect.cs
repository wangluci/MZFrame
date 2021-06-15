
namespace MyNet.MQ.Packet
{
    public class MQDisconnect : MQPacket
    {
        public override byte[] GenerateContent()
        {
            return new byte[0];
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.DISCONNECT << 4;
        }
    }
}
