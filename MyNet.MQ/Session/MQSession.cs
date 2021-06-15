using MyNet.Channel;
using MyNet.Common;
using MyNet.Handlers;
using MyNet.MQ.Customer;
using MyNet.MQ.Packet;
using System;
using System.Collections.Generic;

namespace MyNet.MQ.Session
{
    public class MQSession
    {
        private Dictionary<ushort, MQAckRequest> _sendAckReqs = new Dictionary<ushort, MQAckRequest>();
        private MQBinaryHeap<NotAckedRequest> _notackeds = new MQBinaryHeap<NotAckedRequest>(20);
        private Dictionary<ushort, MQMessage> _publics = new Dictionary<ushort, MQMessage>();
        public Dictionary<ushort, MQMessage> Publics { get { return _publics; } }
        private Dictionary<ushort, MQMessage> _qos2recvs = new Dictionary<ushort, MQMessage>();
        public Dictionary<ushort, MQMessage> Qos2Recvs { get { return _qos2recvs; } }
        private string _sessionid;
        public bool ClearAtClose { get; set; }
        public string SessionId { get { return _sessionid; } }
        /// <summary>
        /// 消费用
        /// </summary>
        public IContext Context { get; set; }
        private ushort _curack;
        public Dictionary<string, MQConsumer> Consumers = new Dictionary<string, MQConsumer>();
        public MQSession(string id)
        {
            ClearAtClose = false;
            _sessionid = id;
        }
        public ushort NextAckId()
        {
            _curack += 1;
            if (_curack >= ushort.MaxValue)
            {
                _curack = 0;
            }
            return _curack;
        }
        public void ResetAckId(ushort newid)
        {
            _curack = newid;
        }


        public void RemoveSendReq(ushort ackid)
        {
            _sendAckReqs.Remove(ackid);
        }

        public void ConsumeMessage(MQMessage msg, byte level)
        {
            MQPublishRequest req = new MQPublishRequest(msg, this.NextAckId());
            if (level == 0)
            {
                Context.Channel.SendAsync(req);
            }
            else
            {
                req.SuccessHandler(OnSuccessHandler);
                req.ErrHandler(OnErrHandler);
                SendNeedAck(req.Ack, req);
            }
        }

        private void OnSuccessHandler(IContext context, WritePacket packet)
        {
            MQPublishRequest mqpk = packet as MQPublishRequest;
            if (mqpk != null)
            {
                this.Publics.Add(mqpk.Ack, mqpk.Message);
            }
        }
        private void OnErrHandler(IContext context, WritePacket packet)
        {
            MQPublishRequest mqpk = packet as MQPublishRequest;
            if (mqpk != null)
            {
                MQSessionManager.Instance().JoinThreadQueue(mqpk.Message, false);
            }
        }
        /// <summary>
        /// 发送需要回复的包
        /// </summary>
        /// <param name="ackid"></param>
        /// <param name="packet"></param>
        public void SendNeedAck(ushort ackid, MQAckRequest packet)
        {
            _sendAckReqs.Add(ackid, packet);
            _notackeds.Enqueue(new NotAckedRequest(ackid, Converter.Cast<long>(DateTime.Now.AddSeconds(5))));
            Context.Channel.SendAsync(packet);
        }
        /// <summary>
        /// 定时重发未回复包
        /// </summary>
        public void ResendNotAcked()
        {
            List<NotAckedRequest> acklist = new List<NotAckedRequest>();
            for (;;)
            {
                NotAckedRequest req = _notackeds.Peek();
                if (req == null || req.OverTime > Converter.Cast<long>(DateTime.Now))
                {
                    break;
                }
                acklist.Add(_notackeds.Dequeue());
            }
            foreach (NotAckedRequest notack in acklist)
            {
                MQAckRequest pack;
                if (_sendAckReqs.TryGetValue(notack.AckId, out pack))
                {
                    pack.IsRepeat = true;
                    SendNeedAck(notack.AckId, pack);
                }
            }
        }

    }
}
