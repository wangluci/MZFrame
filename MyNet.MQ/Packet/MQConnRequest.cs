using MyNet.Buffer;
using System.Text;

namespace MyNet.MQ.Packet
{
    /// <summary>
    /// MQTT3.1.1协议
    /// </summary>
    public class MQConnRequest : MQPacket
    {
        public const string MQTTName = "MQTT";
        private string _username;
        private string _password;
        private string _clientid;
        private bool _clearsession;
        private ushort _idletime;
        public MQConnRequest(string username, string password, ushort idletime, string clientid, bool clearsession)
        {
            _username = username;
            _password = password;
            _clientid = clientid;
            _clearsession = clearsession;
            _idletime = idletime;
        }
        public override byte GenerateHeader()
        {
            return (byte)MQTTType.CONNECT << 4;
        }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();
            //协议名
            byte[] namebytes = Encoding.UTF8.GetBytes(MQTTName);
            stream.WriteShort((short)namebytes.Length);
            stream.WriteBytes(namebytes);
            //协议级别3.1.1
            stream.WriteByte(0x04);
            //连接标志
            byte connsign = 0;
            if (_clearsession)
            {
                connsign |= 0x02;
            }
            if (!string.IsNullOrEmpty(_username))
            {
                connsign |= 0x80;
            }
            if (!string.IsNullOrEmpty(_password))
            {
                connsign |= 0x40;
            }
            stream.WriteByte(connsign);
            stream.WriteShort((short)_idletime);
            stream.WriteShort((short)_clientid.Length);
            if (_clientid.Length > 0)
            {
                stream.WriteBytes(Encoding.UTF8.GetBytes(_clientid));
            }
            if (!string.IsNullOrEmpty(_username))
            {
                stream.WriteShort((short)_username.Length);
                stream.WriteBytes(Encoding.UTF8.GetBytes(_username));
            }
            if (!string.IsNullOrEmpty(_password))
            {
                stream.WriteShort((short)_password.Length);
                stream.WriteBytes(Encoding.UTF8.GetBytes(_password));
            }
            return stream.ToArray();
        }


    }
}
