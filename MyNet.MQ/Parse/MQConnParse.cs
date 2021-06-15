using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using MyNet.MQ.Packet;
using MyNet.MQ.Session;
using System.Text;
namespace MyNet.MQ.Parse
{
    internal class MQConnParse : IMQParse
    {
        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                if (handler.IsConnected)
                {
                    return false;
                }
                ushort len = (ushort)data.ReadShort();
                string tname = Encoding.UTF8.GetString(data.ReadBytes(len));
                if (!MQConnRequest.MQTTName.Equals(tname))
                {
                    return false;
                }
                if (data.ReadByte() != 0x04)
                {
                    context.Channel.SendAsync(new MQConnResponse(ConnCode.ProtocolNoSupport));
                    return true;
                }
                //保留标志
                byte connsign = data.ReadByte();
                if ((connsign & 0x01) != 0)
                {
                    return false;
                }
                //清理会话标志
                bool isclearsession = false;
                if ((connsign & 0x02) != 0)
                {
                    //每次连接使用新会话
                    isclearsession = true;
                }

                //遗嘱标志
                bool will = false;
                if ((connsign & 0x04) != 0)
                {
                    will = true;
                }
                bool enableusername = false;
                //用户名标志
                if ((connsign & 0x80) != 0)
                {
                    enableusername = true;
                }
                bool enablepassword = false;
                //用户名密码
                if ((connsign & 0x40) != 0)
                {
                    enablepassword = true;
                }
                if (!enableusername && enablepassword)
                {
                    return false;
                }
                short idleseconds = data.ReadShort();

                //处理客户端标识
                ushort idlen = (ushort)data.ReadShort();
                string sessionid = string.Empty;
                if (idlen == 0)
                {
                    if (!isclearsession)
                    {
                        context.Channel.SendAsync(new MQConnResponse(ConnCode.ClientIdErr));
                        return true;
                    }
                }
                else
                {
                    sessionid = Encoding.UTF8.GetString(data.ReadBytes(idlen));
                }


                //处理遗嘱
                if (will)
                {
                    //暂不处理
                }
                string username = string.Empty;
                string password = string.Empty;
                if (enableusername)
                {
                    ushort namelen = (ushort)data.ReadShort();
                    username = Encoding.UTF8.GetString(data.ReadBytes(namelen));
                }
                if (enablepassword)
                {
                    ushort passwordlen = (ushort)data.ReadShort();
                    if (passwordlen > 0)
                    {
                        password = Encoding.UTF8.GetString(data.ReadBytes(passwordlen));
                    }
                }
                if (handler.Config.EnableAuth)
                {
                    if (!handler.Config.Serial.IdAuth(username, password))
                    {
                        context.Channel.SendAsync(new MQConnResponse(ConnCode.PasswordOrNameErr));
                        return true;
                    }
                }

                //连接成功处理
                MQSession session;
                if (string.IsNullOrEmpty(sessionid))
                {
                    sessionid = handler.Config.NextId();
                }
                session = MQSessionManager.Instance().NewSession(sessionid, context);
                handler.IsConnected = true;
                session.ClearAtClose = isclearsession;
                handler.Session = session;
                handler.RestartIdleTime(context, (int)(idleseconds * 1.5));
                context.Channel.SendAsync(new MQConnResponse(ConnCode.Success, !isclearsession));

                //监听定时重发
                context.Channel.AddListener(HostConfig.CHANNEL_LOOP_ALIVE, handler.ResendHandler);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            byte sign = data.ReadByte();
            ConnCode code = (ConnCode)data.ReadByte();
            if (code != ConnCode.Success)
            {
                if (handler.Config.Listener != null)
                {
                    string msg = string.Empty;
                    switch (code)
                    {
                        case ConnCode.ProtocolNoSupport:
                            msg = "服务端不支持客户端的协议级别";
                            break;
                        case ConnCode.ClientIdErr:
                            msg = "客户端标识码错误";
                            break;
                        case ConnCode.ServerUnused:
                            msg = "MQTT服务不可用";
                            break;
                        case ConnCode.PasswordOrNameErr:
                            msg = "用户名密码错误";
                            break;
                        default:
                            break;
                    }
                    handler.FinishConn(context, false);
                }
                return false;
            }
            else
            {
                handler.FinishConn(context, true);
                return true;
            }
        }
    }
}
