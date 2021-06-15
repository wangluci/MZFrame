
using MyNet.Middleware.SSL;

namespace MyNet.MQ.Client
{
    public class ClientConfig
    {
        private IMQEventListener _listener;
        public IMQEventListener Listener
        {
            get { return _listener; }
            set { _listener = value; }
        }
        private IMQClientSerialization _serial;
        /// <summary>
        /// 持久化处理
        /// </summary>
        public IMQClientSerialization Serial
        {
            get { return _serial; }
        }
        private string _account;
        /// <summary>
        /// 账号
        /// </summary>
        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }
        private string _password;
        /// <summary>
        /// 客户端连接密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        private int _maxPacketLen;
        /// <summary>
        /// 接收的数据包最大长度
        /// </summary>
        public int MaxPacketLen
        {
            get { return _maxPacketLen; }
        }

        private ushort _pingpongTime;
        /// <summary>
        /// 心跳时间，单位秒
        /// </summary>
        public ushort PingPongTime
        {
            get { return _pingpongTime; }
            set { _pingpongTime = value; }
        }
        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
        private int _port;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private bool _clearSession;
        public bool ClearSession
        {
            get { return _clearSession; }
            set { _clearSession = value; }
        }

        private SSLClientSettings _ssl;
        public SSLClientSettings SSLSettings
        {
            get { return _ssl; }
            set { _ssl = value; }
        }
        public ClientConfig(IMQClientSerialization serial)
        {
            _ssl = null;
            _serial = serial;
            _ip = "127.0.0.1";
            _port = 1883;
            _account = string.Empty;
            _password = string.Empty;
            _maxPacketLen = 1000;
            _pingpongTime = 60;
            _clearSession = true;
        }
    }
}
