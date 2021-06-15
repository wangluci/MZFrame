
namespace MyNet.MQ.Packet
{
    public enum SubsCode : byte
    {
        max_qos0 = 0x00,
        max_qos1 = 0x01,
        max_qos2 = 0x02,
        failure = 0x80
    }
}
