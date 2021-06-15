using MyNet.Buffer;
namespace MyNet.MQ.Packet
{
    public class MQConnResponse : MQPacket
    {
        private ConnCode _code;
        private bool _sessionsave;
        public MQConnResponse(ConnCode code, bool sessionsave)
        {
            _code = code;
            _sessionsave = sessionsave;
        }
        public MQConnResponse(ConnCode code) : this(code, false) { }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();
            if (_sessionsave)
            {
                stream.WriteByte(0x01);
            }
            else
            {
                stream.WriteByte(0x00);
            }
            stream.WriteByte((byte)_code);
            return stream.ToArray();
        }

        public override byte GenerateHeader()
        {
            return (byte)MQTTType.CONNACK << 4;
        }

      
    }
}
