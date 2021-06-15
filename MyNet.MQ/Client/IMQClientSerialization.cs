using System;
namespace MyNet.MQ.Client
{
    public interface IMQClientSerialization
    {
        /// <summary>
        /// 保存发布失败消息
        /// </summary>
        /// <param name="msg"></param>
        void SerialErr(MQMessage msg);
        /// <summary>
        /// 获取所有失败消息
        /// </summary>
        /// <returns></returns>
        MQMessage[] GetAllErr();
    }
}
