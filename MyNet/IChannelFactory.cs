using MyNet.Channel;
using System;
using System.Net;

namespace MyNet
{
    public interface IChannelFactory<T>
    {
        T Create();
        void Remove(ChannelBase c);
        void Close();
    }
}
