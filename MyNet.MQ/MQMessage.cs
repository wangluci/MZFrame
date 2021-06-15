using MyNet.Common.TimeWheel;
using MyNet.MQ.Session;
using System;
namespace MyNet.MQ
{
    public class MQMessage : TimerTask
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 用于判断被发布到哪个队列
        /// </summary>
        public string Topic { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 0最多发一次，1至少发一次，2只发一次
        /// </summary>
        public byte QosLevel { get; set; }

        /// <summary>
        /// 是否保留消息
        /// </summary>
        public bool Retain { get; set; }
        /// <summary>
        /// 触发日期时间
        /// </summary>
        public long TriggeredDate { get; set; }
        /// <summary>
        /// 触发ID
        /// </summary>
        public string TriggerId { get; set; }
        /// <summary>
        /// 所属客户ID
        /// </summary>
        public string ClientId { get; set; }
        public MQMessage Clone()
        {
            MQMessage msg = new MQMessage();
            msg.MessageId = MessageId;
            msg.Topic = Topic;
            msg.Content = Content;
            msg.QosLevel = QosLevel;
            msg.Retain = Retain;
            msg.TriggeredDate = TriggeredDate;
            msg.TriggerId = TriggerId;
            msg.ClientId = ClientId;
            return msg;
        }
        public void Run(IWheelTimeout timeout)
        {
            if (!string.IsNullOrEmpty(this.TriggerId))
            {
                MQSessionManager.Instance().NoTrigger(this.TriggerId);
            }
            MQSessionManager.Instance().RouteMessage(this);
        }
    }
}
