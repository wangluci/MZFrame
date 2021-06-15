
using System;

namespace MyNet.MQ.Packet
{
    public abstract class MQAckRequest : MQPacket
    {
        protected bool _isrepeat;
        public bool IsRepeat
        {
            get { return _isrepeat; }
            set { _isrepeat = value; }
        }
        public MQAckRequest()
        {
            _isrepeat = false;
        }
    }
}
