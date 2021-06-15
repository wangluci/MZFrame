using MyNet.MQ.Packet;

namespace MyNet.MQ.Client
{
    /// <summary>
    /// 事件监听器
    /// </summary>
    public interface IMQEventListener
    {
        /// <summary>
        /// 消费事件处理
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        void ConsumeHandle(MQMessage msg);
    }
}
