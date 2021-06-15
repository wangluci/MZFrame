using System.Collections.Generic;

namespace MyNet.MQ
{
    public interface IMQServerSerialization
    {
        bool IdAuth(string username, string password);
        /// <summary>
        /// 加载消息进行初始化
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        MQMessage[] LoadInitData(int i, int total);
        /// <summary>
        /// 串行化指定消息
        /// </summary>
        /// <param name="message"></param>
        void SerialMessage(MQMessage message);

        /// <summary>
        /// 指定消息成功
        /// </summary>
        /// <param name="messageid"></param>
        void SuccessMessage(string messageid);
    }
}
