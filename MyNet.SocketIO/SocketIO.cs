using MyNet.Middleware.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyNet.SocketIO
{
    /// <summary>
    /// 全局配置类 本插件目前只支持SOCKET.IO 2.0客户端版本
    /// </summary>
    public class SocketIO
    {
        /// <summary>
        /// Socket.io的Path
        /// </summary>
        private string _path = "/socket.io/";
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        /// <summary>
        /// Ping过期时间，单位毫秒
        /// </summary>
        private int _pingTimeout = 10000;
        public int PingTimeout
        {
            get { return _pingTimeout; }
            set { _pingTimeout = value; }
        }
        /// <summary>
        /// Ping过期时间，单位毫秒
        /// </summary>
        private int _pingInterval = 25000;
        public int PingInterval
        {
            get { return _pingInterval; }
            set { _pingInterval = value; }
        }
        /// <summary>
        /// 升级传输协议过期时间，单位毫秒
        /// </summary>
        private int _upgradeTimeout = 10000;
        public int UpgradeTimeout
        {
            get { return _upgradeTimeout; }
            set { _upgradeTimeout = value; }
        }
        /// <summary>
        /// 默认命名空间
        /// </summary>
        public const string DEFAULT_NAME = "/";
        public const string WEBSOCKET_NAME = "websocket";
        public const string POLLING_NAME = "polling";
        private static SocketIO mSingleton;
        private static readonly object lockHelper = new object();
        private ConcurrentDictionary<string, IOClient> _clients = new ConcurrentDictionary<string, IOClient>();
        /// <summary>
        /// 全名空间管理中心
        /// </summary>
        private NamespacesHub _nsHub;
        public NamespacesHub NSHub
        {
            get { return _nsHub; }
        }

        /// <summary>
        /// 添加默认空间的连接监听
        /// </summary>
        /// <param name="listener"></param>
        public void AddConnectListener(ConnectionListener listener)
        {
            Namespace ns = _nsHub.Get(DEFAULT_NAME);
            if (ns != null)
            {
                ns.AddConnectListener(listener);
            }
        }
        public SocketIO()
        {
            _nsHub = new NamespacesHub();
            _nsHub.Create(DEFAULT_NAME);
        }
        public static HttpResponse CreateResponse(string sessionid, string origin)
        {
            HttpResponse response = new HttpResponse();
            if (!string.IsNullOrEmpty(sessionid))
            {
                response.Headers.Add("set-cookie", string.Format("io={0}", sessionid));
            }
            if (!string.IsNullOrEmpty(origin))
            {
                response.Headers.Add("access-control-allow-origin", origin);
                response.Headers.Add("access-control-allow-credentials", "true");
            }
            else
            {
                response.Headers.Add("access-control-allow-origin", "*");
            }
            return response;
        }
        public static Transport ParseTransport(string transport)
        {
            switch (transport)
            {
                case WEBSOCKET_NAME:
                    return Transport.WEBSOCKET;
                case POLLING_NAME:
                    return Transport.POLLING;
                default:
                    return Transport.UNKNOWN;
            }
        }
        public static SocketIO Instance()
        {
            if (mSingleton == null)
            {
                lock (lockHelper)
                {
                    if (mSingleton == null)
                    {
                        mSingleton = new SocketIO();
                    }
                }
            }
            return mSingleton;
        }
        public void AllDisconnect()
        {
            foreach (KeyValuePair<string, IOClient> kvp in _clients)
            {
                kvp.Value.Disconnect();
            }
        }
        internal void AddClient(string id, IOClient client)
        {
            _clients.TryAdd(id, client);
        }
        internal IOClient RemoveClient(string id)
        {
            IOClient rt;
            if (_clients.TryRemove(id, out rt))
            {
                return rt;
            }
            else
            {
                return null;
            }
        }

        internal IOClient GetClient(string id)
        {
            IOClient client;
            if (_clients.TryGetValue(id, out client))
            {
                return client;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通过id获取默认命名空间nsclient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NSClient GetDefaultNSClient(string id)
        {
            IOClient ioclient = GetClient(id);
            if (ioclient != null)
            {
                return ioclient.GetNSClient(DEFAULT_NAME);
            }
            else
            {
                return null;
            }
        }
    }
    public enum Transport
    {
        WEBSOCKET,
        POLLING,
        UNKNOWN
    }
}
