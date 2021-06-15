using MyNet.Handlers;
using System.Collections.Generic;
using System;
using System.Threading;
using MyNet.Common;
using System.Collections.Concurrent;

namespace MyNet.SocketIO
{
    public delegate void IODisconnectHandler();
    /// <summary>
    /// 真正连接客户端
    /// </summary>
    internal class IOClient
    {
        private Transport _transport;
        public Transport CurentTransport
        {
            get { return _transport; }
        }

        private string _sessionid;
        public string SessionID
        {
            get { return _sessionid; }
        }
        private ReaderWriterLockSlim _lockslim = new ReaderWriterLockSlim();
        private ConcurrentQueue<SocketIOPacket> _ioPollPacketQueues;
        private ConcurrentQueue<SocketIOPacket> _ioSocketPacketQueues;
        private ConcurrentDictionary<string, EmitWorker> _handlers = new ConcurrentDictionary<string, EmitWorker>();
        private Dictionary<string, NSClient> _namespaceclients = new Dictionary<string, NSClient>();
        private Dictionary<Transport, TransportState> _transports = new Dictionary<Transport, TransportState>();
        private SocketIOPacket _lastBinaryPacket;
        private System.Timers.Timer _openTimer;
        private System.Timers.Timer _pingTimer;
        private System.Timers.Timer _upgradeTimer;
        private int _conncount = 0;
        private long _initAckId = 0;
        private ReaderWriterLockSlim _uprwlock = new ReaderWriterLockSlim();

        private IOClient(Transport transport, string sessionid)
        {
            _ioPollPacketQueues = new ConcurrentQueue<SocketIOPacket>();
            _ioSocketPacketQueues = new ConcurrentQueue<SocketIOPacket>();
            _transport = transport;
            _sessionid = sessionid;
            _transports.Add(Transport.POLLING, new PollingState(_ioPollPacketQueues));
            _transports.Add(Transport.WEBSOCKET, new WebsocketState(_ioSocketPacketQueues));
            //定时判断是否连接,5秒内不连接则删除
            _openTimer = new System.Timers.Timer();
            _openTimer.AutoReset = false;
            _openTimer.Interval = 5000;
            _openTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
            {
                SocketIO.Instance().RemoveClient(sessionid);
                Disconnect();
            });
            _openTimer.Start();
        }
        public static IOClient Create(Transport transport, string sessionid)
        {
            IOClient client = new IOClient(transport, sessionid);
            SocketIO.Instance().AddClient(sessionid, client);
            return client;
        }

        public void Connect()
        {
            if (Interlocked.CompareExchange(ref _conncount, 1, 0) == 0)
            {
                if (_openTimer != null)
                {
                    _openTimer.Stop();
                    _openTimer.Dispose();
                    _openTimer = null;
                }
                //触发连接事件
                Namespace ns = SocketIO.Instance().NSHub.Get(SocketIO.DEFAULT_NAME);
                if (ns != null && !ns.Contain(this.SessionID))
                {
                    SocketIOPacket packet = new SocketIOPacket(PacketType.MESSAGE);
                    packet.SetSubType(SubPacketType.CONNECT);
                    packet.SetNsp(ns.Name);
                    this.ConnectNamespace(ns);
                    this.Send(packet);
                }
            }
            else
            {
                if (_transport == Transport.WEBSOCKET)
                {
                    Send(Transport.POLLING, new SocketIOPacket(PacketType.NOOP));
                }
            }
        }
        /// <summary>
        /// 连接时添加命名空间
        /// </summary>
        /// <param name="ns"></param>
        public void ConnectNamespace(Namespace ns)
        {
            NSClient nsclient = new NSClient(ns, this);
            _lockslim.EnterWriteLock();
            try
            {
                _namespaceclients.Add(ns.Name, nsclient);
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
            ns.EmitConnect(nsclient);
        }
        public TransportState GetTransportState(Transport trans)
        {
            return _transports[trans];
        }

        public long IncreaseAckId()
        {
            return Interlocked.Increment(ref _initAckId);
        }
        /// <summary>
        /// 升级当前的传输协议
        /// </summary>
        /// <param name="transport"></param>
        public void UpgradeCurrentTransport()
        {
            _uprwlock.EnterWriteLock();
            try
            {
                _transport = Transport.WEBSOCKET;
                SocketIOPacket packet;
                while (_ioPollPacketQueues.TryDequeue(out packet))
                {
                    _ioSocketPacketQueues.Enqueue(packet);
                }
            }
            finally
            {
                _uprwlock.ExitWriteLock();
            }
            ReplySocketIOData(Transport.WEBSOCKET);
        }

        public void SetContext(Transport trans, IContext context)
        {
            _transports[trans].UpdateContext(context);
        }

        /// <summary>
        /// 主动与客户端断开连接
        /// </summary>
        public void Disconnect()
        {
            int originalState = Interlocked.CompareExchange(ref _conncount, 0, 1);
            if (originalState != 1)
            {
                return;
            }
            SocketIOPacket packet = new SocketIOPacket(PacketType.MESSAGE);
            packet.SetSubType(SubPacketType.DISCONNECT);
            Send(packet);
            CancelUpgradeTimeout();
            NSClient[] klist;
            _lockslim.EnterReadLock();
            try
            {
                klist = new NSClient[_namespaceclients.Keys.Count];
                _namespaceclients.Values.CopyTo(klist, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (klist != null)
            {
                foreach (NSClient nc in klist)
                {
                    nc.Disconnect();
                }
            }
            _handlers.Clear();
            SocketIO.Instance().RemoveClient(_sessionid);
            CancelPingTimeout();
        }

        public void AddListener(string name, Action<IOEventArgs> action, bool async)
        {
            EmitWorker w;
            if (_handlers.TryGetValue(name, out w))
            {
                w.AppendAction(action);
            }
            else
            {
                _handlers.TryAdd(name, new EmitWorker(async, action));
            }
        }
        public void Emit(string name, IOEventArgs e)
        {
            EmitWorker w;
            if (_handlers.TryGetValue(name, out w))
            {
                w.Run(e);
            }
        }
        public void RemoveNSClient(NSClient nsclient)
        {
            bool needdisconnect = false;
            _lockslim.EnterWriteLock();
            try
            {
                if (_namespaceclients.Remove(nsclient.NameSpace.Name))
                {
                    if (_namespaceclients.Count <= 0)
                    {
                        needdisconnect = true;
                    }
                }
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
            if (needdisconnect)
            {
                Disconnect();
            }
        }
        public NSClient GetNSClient(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                NSClient nsc;
                if (_namespaceclients.TryGetValue(ns, out nsc))
                {
                    return nsc;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }


        /// <summary>
        /// 清除升级过期定时器
        /// </summary>
        public void CancelUpgradeTimeout()
        {
            if (_upgradeTimer != null)
            {
                _upgradeTimer.Stop();
                _upgradeTimer.Dispose();
                _upgradeTimer = null;
            }
        }
        public void ResetUpgradeTimeout()
        {
            int timeout = SocketIO.Instance().UpgradeTimeout;
            if (_upgradeTimer == null)
            {
                _upgradeTimer = new System.Timers.Timer();
                _upgradeTimer.AutoReset = false;
                _upgradeTimer.Interval = timeout;
                _upgradeTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    Disconnect();
                    CancelUpgradeTimeout();
                });
                _upgradeTimer.Start();
            }
            else
            {
                _upgradeTimer.Stop();
                _upgradeTimer.Interval = timeout;
                _upgradeTimer.Start();
            }
        }
        /// <summary>
        /// 清除心跳过期定时器
        /// </summary>
        public void CancelPingTimeout()
        {
            if (_pingTimer != null)
            {
                _pingTimer.Stop();
                _pingTimer.Dispose();
                _pingTimer = null;
            }
        }
        public void ResetPingTimeout()
        {
            int timeout = SocketIO.Instance().PingTimeout + SocketIO.Instance().PingInterval;
            if (_pingTimer == null)
            {
                _pingTimer = new System.Timers.Timer();
                _pingTimer.AutoReset = false;
                _pingTimer.Interval = timeout;
                _pingTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    Disconnect();
                    CancelPingTimeout();
                });
                _pingTimer.Start();
            }
            else
            {
                _pingTimer.Stop();
                _pingTimer.Interval = timeout;
                _pingTimer.Start();
            }
        }

        public void SetLastBinaryPacket(SocketIOPacket packet)
        {
            _lastBinaryPacket = packet;
        }
        public SocketIOPacket GetLastBinaryPacket()
        {
            return _lastBinaryPacket;
        }
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet"></param>
        public void Send(SocketIOPacket packet)
        {
            _uprwlock.EnterReadLock();
            try
            {
                if (_transport == Transport.POLLING)
                {
                    _ioPollPacketQueues.Enqueue(packet);
                }
                else
                {
                    _ioSocketPacketQueues.Enqueue(packet);
                }
            }
            finally
            {
                _uprwlock.ExitReadLock();
            }
            _transports[_transport].Send(_sessionid);
        }
        public void Send(Transport transport, SocketIOPacket packet)
        {
            if (transport == Transport.POLLING)
            {
                _ioPollPacketQueues.Enqueue(packet);
            }
            else
            {
                _ioSocketPacketQueues.Enqueue(packet);
            }
            _transports[transport].Send(_sessionid);
        }
        public TransportState GetTransport()
        {
            return _transports[_transport];
        }
        /// <summary>
        /// 触发发送数据包（使用指定传输通道）
        /// </summary>
        /// <param name="transport"></param>
        public void ReplySocketIOData(Transport transport)
        {
            _transports[transport].Send(_sessionid);
        }
    }
}
