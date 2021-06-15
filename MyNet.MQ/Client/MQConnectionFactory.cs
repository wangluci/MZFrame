using MyNet.Channel;
using MyNet.ClientConnection;
using MyNet.Middleware.SSL;
using System;

namespace MyNet.MQ.Client
{
    public class MQConnectionFactory : AbstractConnectionFactory<MQConnection>
    {
        protected int _trycount = 0;
        protected DateTime _lasttry;
        protected ClientConfig _config;
        public ClientConfig Config
        {
            get { return _config; }
        }

        public MQConnectionFactory(ClientConfig config) : base(config.Ip, config.Port, string.Empty)
        {
            _config = config;
            _bootstrap.Handler(new InitializerHandler(c =>
            {
                if (config.SSLSettings != null)
                {
                    c.Pipeline.AddLast(new SSLHandler(config.SSLSettings));
                }
                c.Pipeline.AddLast(new MQConnectionHandler(config));
            }));
        }
        public override MQConnection NewConnection(string key)
        {
            MQConnection conn = new MQConnection(key, _bootstrap.LaunchChannel(_ip, _port));
            if (conn.Active)
            {
                if (_trycount <= 3)
                {
                    //创建临时连接处理发布失败的事件包
                    bool reserial = true;
                    MQMessage[] list = _config.Serial.GetAllErr();
                    if (list.Length > 0)
                    {
                        ++_trycount;
                        _lasttry = DateTime.Now;
                        MQConnection tmpconn = new MQConnection(key, _bootstrap.LaunchChannel(_ip, _port));
                        if (tmpconn.Active)
                        {
                            reserial = false;
                            tmpconn.Connect(string.Empty);
                            foreach (MQMessage e in list)
                            {
                                tmpconn.Publish(e);
                            }
                            tmpconn.Close();
                            _trycount = 0;
                        }
                    }
                    if (reserial)
                    {
                        foreach (MQMessage m in list)
                        {
                            _config.Serial.SerialErr(m);
                        }
                    }
                }
                else
                {
                    //1小时后在尝试
                    if (_lasttry.AddHours(1) < DateTime.Now)
                    {
                        _trycount = 0;
                    }
                }
            }
            return conn;
        }
    }
}
