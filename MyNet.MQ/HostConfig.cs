using MyNet.Middleware.SSL;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MyNet.MQ
{
    /// <summary>
    /// 主机配置信息
    /// </summary>
    public class HostConfig
    {
        public const string CHANNEL_LOOP_ALIVE = "my:loopalive";
        private IMQServerSerialization _serial;
        /// <summary>
        /// 持久化处理
        /// </summary>
        public IMQServerSerialization Serial
        {
            get { return _serial; }
        }

        private int _workcount;
        /// <summary>
        /// 连接处理线程数
        /// </summary>
        public int Workcount {
            get { return _workcount; }
            set { _workcount = value; }
        }
        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                ResetIdGenerater();
            }
        }
        private int _port;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                ResetIdGenerater();
            }
        }

        private bool _enableauth;
        /// <summary>
        /// 是否启用身份认证
        /// </summary>
        public bool EnableAuth
        {
            get { return _enableauth; }
            set { _enableauth = value; }
        }
        /// <summary>
        /// 每个连接的延时处理间隔
        /// </summary>
        private int _loopAliveMilliSeconds;
        public int LoopAliveMilliSeconds
        {
            get { return _loopAliveMilliSeconds; }
            set { _loopAliveMilliSeconds = value; }
        }
        private IdGenerater _idGenerater;
        private int _delayQueueCount;
        /// <summary>
        /// 延迟队列数
        /// </summary>
        public int DelayQueueCount
        {
            get { return _delayQueueCount; }
            set { _delayQueueCount = value; }
        }
        private void ResetIdGenerater()
        {
            char[] separator = new char[] { '.' };
            string[] items = Ip.Split(separator);
            long lgip = long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
            _idGenerater = new IdGenerater(Port, lgip.ToString());
        }
        private SSLServerSettings _ssl;
        /// <summary>
        /// SSL证书，null则不使用
        /// </summary>
        public SSLServerSettings SSLSettings
        {
            get { return _ssl; }
            set { _ssl = value; }
        }
        public HostConfig(IMQServerSerialization serial)
        {
            _ssl = null;
            _delayQueueCount = 2;
            _serial = serial;
            _workcount = 4;
            _ip = "127.0.0.1";
            _port = 1883;
            _enableauth = true;
            _loopAliveMilliSeconds = 2000;
            ResetIdGenerater();
        }
        public string NextId()
        {
            return _idGenerater.nextId();
        }
    }
}
