using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.MQ.Client;
using System.Text;
using MyNet.MQ.Packet;
using MyNet.MQ.Session;

namespace MyNet.MQ.Parse
{
    internal class MQPublishParse : IMQParse
    {
        public bool ClientParse(IContext context, IByteStream data, MQConnectionHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = 0;
                ushort len = (ushort)data.ReadShort();
                string topic = Encoding.UTF8.GetString(data.ReadBytes(len));
                if (qos == 1 || qos == 2)
                {
                    ack = (ushort)data.ReadShort();
                }
                int contlen = data.Length - data.ReaderIndex;
                byte[] contbytes = data.ReadBytes(contlen);

                MQMessage message = new MQMessage();
                message.QosLevel = qos;
                message.Retain = retain;
                message.Topic = topic;
                message.Content = Encoding.UTF8.GetString(contbytes);
                message.ClientId = string.Empty;
                if (qos == 0)
                {
                    handler.FinishNotice(context, message);
                }
                else if (qos == 1)
                {
                    handler.FinishNotice(context, message);
                    context.Channel.SendAsync(new MQPublishResponse(ack));
                }
                else if (qos == 2)
                {
                    if (!handler.Recvs.ContainsKey(ack))
                    {
                        handler.Recvs.Add(ack, message);
                    }
                    context.Channel.SendAsync(new MQPubrecRequest(ack));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ServerParse(IContext context, IByteStream data, MQHandler handler, bool dup, byte qos, bool retain)
        {
            try
            {
                ushort ack = 0;
                ushort len = (ushort)data.ReadShort();
                string topic = Encoding.UTF8.GetString(data.ReadBytes(len));
                if (qos == 1 || qos == 2)
                {
                    ack = (ushort)data.ReadShort();
                }
                int contlen = data.Length - data.ReaderIndex;
                byte[] contbytes = data.ReadBytes(contlen);

                MQMessage message = new MQMessage();
                message.QosLevel = qos;
                message.Retain = retain;
                message.Topic = topic;
                message.ClientId = string.Empty;
                message.Content = Encoding.UTF8.GetString(contbytes);
                if (message.Content.StartsWith("T["))
                {
                    string tcont = message.Content.Substring(2);
                    int endi = tcont.IndexOf("]");
                    if (endi > 0)
                    {
                        string tt = tcont.Substring(0, endi);
                        string[] ttarr = tt.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                        if (ttarr.Length > 0)
                        {
                            long rt = -1;
                            if (long.TryParse(ttarr[0], out rt))
                            {
                                message.TriggeredDate = rt;
                                message.Content = tcont.Substring(endi + 1);
                            }
                        }
                        if (ttarr.Length > 1)
                        {
                            message.TriggerId = ttarr[1];
                        }
                    }
                }

                if (qos == 0)
                {
                    if (string.IsNullOrEmpty(message.Topic))
                    {
                        MQSessionManager.Instance().NoTrigger(message.Content);
              
                    }
                    else
                    {
                        MQSessionManager.Instance().RouteMessage(message);
                    }

                }
                else if (qos == 1)
                {
                    if (string.IsNullOrEmpty(message.Topic))
                    {
                        MQSessionManager.Instance().NoTrigger(message.Content);
                    }
                    else
                    {
                        MQSessionManager.Instance().RouteMessage(message);
                    }
                    context.Channel.SendAsync(new MQPublishResponse(ack));
                }
                else if (qos == 2)
                {
                    if (!handler.Session.Qos2Recvs.ContainsKey(ack))
                    {
                        handler.Session.Qos2Recvs.Add(ack, message);
                    }
                    context.Channel.SendAsync(new MQPubrecRequest(ack));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
