using MyNet.Buffer;
using System.Text;

namespace MyNet.MQ.Packet
{
    public class MQPublishRequest : MQAckRequest
    {
        /// <summary>
        /// 报文标识，每个连接累加
        /// </summary>
        private ushort _ack;
        private MQMessage _message;
        public MQMessage Message { get { return _message; } }
        public ushort Ack { get { return _ack; } }
        public MQPublishRequest(MQMessage message, ushort ack)
        {
            _message = message;
            _ack = ack;
        }
        public override byte[] GenerateContent()
        {
            IByteStream stream = PoolBufferAllocator.Default.AllocStream();

            //主题
            byte[] topic = Encoding.UTF8.GetBytes(_message.Topic);
            stream.WriteShort((short)topic.Length);
            stream.WriteBytes(topic);
            if (_message.QosLevel == 1 || _message.QosLevel == 2)
            {
                //报文标识
                stream.WriteShort((short)_ack);
            }
            byte[] contbytes = Encoding.UTF8.GetBytes(_message.Content);
            stream.WriteBytes(contbytes);
            return stream.ToArray();
        }

        public override byte GenerateHeader()
        {
            byte hdsign = 0;
            if (_message.Retain)
            {
                hdsign |= 0x01;
            }
            if (_isrepeat)
            {
                hdsign |= 0x08;
            }
            if (_message.QosLevel == 1)
            {
                hdsign |= 0x02;
            }
            else if (_message.QosLevel == 2)
            {
                hdsign |= 0x04;
            }
            return (byte)(((byte)MQTTType.PUBLISH << 4) | hdsign);
        }

    }
}
