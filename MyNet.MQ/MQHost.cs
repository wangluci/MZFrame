using MyNet.Channel;
using MyNet.Handlers;
using MyNet.Middleware.SSL;
using MyNet.MQ.Session;
using System.Threading;

namespace MyNet.MQ
{
    /// <summary>
    /// 主机
    /// </summary>
    public class MQHost
    {
        private int _enablestart;
        private ServerBootstrap _boot;
        private HostConfig _config;
        private EventLoopGroup _acceptGroup;
        private EventLoopGroup _workGroup;
        private ChannelBase _acceptChannel;
        public HostConfig Config
        {
            get { return _config; }
        }
        public MQHost(HostConfig config)
        {
            _enablestart = 0;
            _config = config;
            _boot = new ServerBootstrap();
            _boot.Handler(new InitializerHandler(c =>
            {
                if (_config.SSLSettings != null)
                {
                    c.Pipeline.AddLast(new SSLHandler(_config.SSLSettings));
                }
                c.Pipeline
                .AddLast(new LoopTimeoutHandler(config.LoopAliveMilliSeconds, HostConfig.CHANNEL_LOOP_ALIVE))
                .AddLast(new MQHandler(config));
            }));
        }
        public void StartService()
        {
            int rt = Interlocked.Increment(ref _enablestart);
            if (rt > 1) return;
            MQSessionManager.Instance().Init(_config);
            _acceptGroup = new EventLoopGroup();
            _workGroup = new EventLoopGroup(_config.Workcount);
            _boot.Group(_acceptGroup, _workGroup);
            _acceptChannel = _boot.LaunchChannel(_config.Ip, _config.Port);
        }
        public void StopService()
        {
            _acceptChannel = null;
            ServerChannelFactory serverfactory = _boot.Factory() as ServerChannelFactory;
            if (serverfactory != null)
            {
                ChannelBase[] channels = serverfactory.CG.ServerArray();
                foreach (ChannelBase c in channels)
                {
                    c.Dispose();
                }
            }
            if (_acceptGroup != null)
            {
                _acceptGroup.Dispose();
                _acceptGroup = null;
            }
            if (_workGroup != null)
            {
                _workGroup.Dispose();
                _workGroup = null;
            }
            _boot.Group(null, null);
            MQSessionManager.Instance().Dispose();
            Interlocked.Decrement(ref _enablestart);
        }
    }
}
