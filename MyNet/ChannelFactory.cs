using System;
using MyNet.Channel;

namespace MyNet
{
    /// <summary>
    /// 客户端连接工厂
    /// </summary>
    public class ChannelFactory : IChannelFactory<TcpChannel>
    {
        public void Close(){}

        public TcpChannel Create()
        {
            return new TcpSocketChannel();
        }

        public void Remove(ChannelBase c){}
    }
}
