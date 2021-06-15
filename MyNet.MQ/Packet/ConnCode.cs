namespace MyNet.MQ.Packet
{
    public enum ConnCode : byte
    {
        Success = 0x0, //连接成功
        ProtocolNoSupport = 0x01,//协议不支持
        ClientIdErr = 0x02,//客户端标识错误
        ServerUnused = 0x03,//服务不可用
        PasswordOrNameErr = 0x04//用户名或密码错误
    }
}
