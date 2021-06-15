using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using System.Text;

namespace MyNet.MQ.Packet
{
    public abstract class MQPacket : WritePacket
    {
        public void GenerateWritePacket()
        {
            _stream.WriteByte(GenerateHeader());
            byte[] contentbytes = GenerateContent();
            int tlen = contentbytes.Length;
            byte encodedByte;
            do
            {
                encodedByte = (byte)(tlen % 128);
                tlen = tlen / 128;
                if (tlen > 0)
                {
                    encodedByte = (byte)(encodedByte | 128);
                }
                _stream.WriteByte(encodedByte);
            } while (tlen > 0);
            _stream.WriteBytes(contentbytes);
        }
        public abstract byte GenerateHeader();
        public abstract byte[] GenerateContent();
    }
}
