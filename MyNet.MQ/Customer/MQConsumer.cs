
namespace MyNet.MQ.Customer
{
    public class MQConsumer
    {
        private Topic _topicFilter;
        public Topic TopicFilter { get { return _topicFilter; } }
        private string _sessionid;
        public string SessionId { get { return _sessionid; } }
        /// <summary>
        /// 消费级别
        /// </summary>
        private byte _qos;
        public byte Qos { get { return _qos; } }
        public MQConsumer(string sessionid, Topic topicFilter, byte qos)
        {
            _topicFilter = topicFilter;
            _sessionid = sessionid;
            _qos = qos;
        }
        public MQConsumer(MQConsumer orig)
        {
            this._qos = orig._qos;
            this._sessionid = orig._sessionid;
            this._topicFilter = orig._topicFilter;
        }

        //public bool Consume(MQMessage message)
        //{
        //    message.Retain = false;
        //    message.QosLevel = Math.Min(message.QosLevel, _qos);
        //    return MQSessionManager.Instance().SessionConsume(_sessionid, message);
        //}
        public bool QosLessThan(MQConsumer sub)
        {
            return _qos < sub._qos;
        }
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            MQConsumer that = obj as MQConsumer;
            if (that == null) return false;

            if (_sessionid != null ? !_sessionid.Equals(that._sessionid) : that._sessionid != null)
                return false;
            return !(_topicFilter != null ? !_topicFilter.Equals(that._topicFilter) : that._topicFilter != null);
        }
        public override int GetHashCode()
        {
            int result = _sessionid != null ? _sessionid.GetHashCode() : 0;
            result = 31 * result + (_topicFilter != null ? _topicFilter.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return string.Format("[filter:{0}, clientID: {1}, qos: {2}]", _topicFilter.ToString(), _sessionid, _qos);
        }

    }
}
