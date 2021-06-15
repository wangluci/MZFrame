using MyNet.Common;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using System;
using System.Threading;

namespace MyNet.SocketIO
{
    /// <summary>
    /// 命名空间客户端代理类
    /// </summary>
    public class NSClient
    {
        private Namespace _ns;
        private IOClient _client;
        private int _connstate = 1;
        public bool IsConnected
        {
            get
            {
                return _connstate == 1;
            }
        }
        public string ClientId { get { return _client.SessionID; } }
        public const string DISCONNECT_EVENT = "disconnect";
        internal NSClient(Namespace ns, IOClient client)
        {
            _ns = ns;
            _client = client;
            ns.AddClient(this);
        }
        /// <summary>
        /// 客户端附加信息
        /// </summary>
        private object _attachInfo;
        public void SetAttachInfo(object attach)
        {
            _attachInfo = attach;
        }
        public T CastAttach<T>() where T : class
        {
            return _attachInfo as T;
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string name, Action<IOEventArgs> ac, bool async = false)
        {
            _client.AddListener(string.Format("ns:{0}/{1}", _ns.Name, name), (IOEventArgs e) =>
            {
                if (e != null)
                {
                    try
                    {
                        ac(e);
                    }
                    catch (Exception ex)
                    {
                        Common.AgentLogger.Instance.Err(ex.Message);
                    }
                    e.Ask(null);
                }
            }, async);
        }
        /// <summary>
        /// 监听断开连接
        /// </summary>
        /// <param name="handler"></param>
        public void AddDisconnectListener(IODisconnectHandler handler, bool async = false)
        {
            _client.AddListener(string.Format("ns:{0}/{1}", _ns.Name, DISCONNECT_EVENT), (IOEventArgs e) =>
            {
                handler();
            }, async);
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="e"></param>
        internal void EmitEvent(string name, IOEventArgs e)
        {
            _client.Emit(string.Format("ns:{0}/{1}", _ns.Name, name), e);
        }
        /// <summary>
        /// 触发指定ackid的事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ackid"></param>
        /// <param name="e"></param>
        internal void EmitEvent(string name, long ackid, IOEventArgs e)
        {
            _client.Emit(string.Format("ns:{0}/{1}:{2}", _ns.Name, name, ackid), e);
        }
        /// <summary>
        /// 触发断开连接事件
        /// </summary>
        internal void EmitDisconnect()
        {
            _client.Emit(string.Format("ns:{0}/{1}", _ns.Name, DISCONNECT_EVENT), null);
        }
        /// <summary>
        /// 通知，无返回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Notice(string name, object data)
        {
            SocketIOPacket sendpacket = new SocketIOPacket(PacketType.MESSAGE);
            sendpacket.SetSubType(SubPacketType.EVENT);
            sendpacket.SetName(name);
            sendpacket.SetData(data);
            sendpacket.SetNsp(_ns.Name);
            _client.Send(sendpacket);
        }
        /// <summary>
        /// 发送事件给客户端，有返回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="success"></param>
        /// <param name="err"></param>
        public void Emit(string name, object data, bool async = false)
        {
            SocketIOPacket sendpacket = new SocketIOPacket(PacketType.MESSAGE);
            sendpacket.SetSubType(SubPacketType.EVENT);
            sendpacket.SetName(name);
            sendpacket.SetData(data);
            sendpacket.SetAckId(_client.IncreaseAckId());
            sendpacket.SetNsp(_ns.Name);
            _client.Send(sendpacket);
        }
        public void Disconnect()
        {
            int originalState = Interlocked.CompareExchange(ref _connstate, 0, 1);
            if (originalState != 1)
            {
                return;
            }
            EmitDisconnect();
            _client.RemoveNSClient(this);
            _ns.RemoveClient(_client.SessionID);
        }

        internal IOClient Client
        {
            get { return _client; }
        }
        public Namespace NameSpace
        {
            get { return _ns; }
        }
    }
}
