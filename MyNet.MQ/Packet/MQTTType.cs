
namespace MyNet.MQ.Packet
{
    public enum MQTTType : byte
    {
        Reserved,
        CONNECT,//客户端请求连接到服务器
        CONNACK,//连接确认
        PUBLISH,//发布消息
        PUBACK,//发布确认
        PUBREC,//发布收到
        PUBREL,//发布释放
        PUBCOMP,//发布完成
        SUBSCRIBE,//客户端请求订阅
        SUBACK,//订阅确认
        UNSUBSCRIBE,//请求取消订阅
        UNSUBACK,//取消订阅确认
        PINGREQ,//PING请求
        PINGRESP,//PING应答
        DISCONNECT//中断连接
    }
}
