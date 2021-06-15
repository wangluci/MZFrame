using MyNet.Buffer;
using MyNet.Common;
using MyNet.Handlers;
using MyNet.Middleware.Http;
using MyNet.Middleware.Http.WebSocket;
using System;
using System.Text;

namespace MyNet.SocketIO
{
    public class SocketIOHandler : AbstractChannelHandler
    {
        private IOClient _curclient;
        public SocketIOHandler()
        {
        }
        public override void ChannelRead(IContext context, object msg)
        {
            HttpRequest request = msg as HttpRequest;
            if (request != null)
            {
                //验证socketio
                if (!string.Equals(request.Path, SocketIO.Instance().Path, StringComparison.OrdinalIgnoreCase))
                {
                    context.FireNextRead(msg);
                    return;
                }
                string origin = request.Headers.TryGet<string>("origin");
                context.Channel.Propertys.TryAdd("origin", origin);
                bool b64 = request.Query.TryGet("b64", false);
                context.Channel.Propertys.TryAdd("b64", b64);
                string sid = request.Query.Get("sid");
                if (sid == null)
                {
                    string sessionid = request.Headers.TryGet<string>("io");
                    if (string.IsNullOrEmpty(sessionid))
                    {
                        sessionid = Guid.NewGuid().ToString("N");
                    }
                    string transport = request.Query.Get("transport");
                    Transport trans = SocketIO.ParseTransport(transport);
                    if (trans == Transport.UNKNOWN)
                    {
                        SocketIO.CreateResponse(null, origin).End401(context);
                        return;
                    }
                    //发送握手包
                    AuthData authdata = new AuthData();
                    authdata.sid = sessionid;
                    authdata.upgrades = new string[] { "websocket" };
                    authdata.pingInterval = SocketIO.Instance().PingInterval;
                    authdata.pingTimeout = SocketIO.Instance().PingTimeout;
                    SocketIOPacket iopacket = new SocketIOPacket(PacketType.OPEN);
                    iopacket.SetData(authdata);
                    _curclient = IOClient.Create(trans, sessionid);
                    TransportState ts = _curclient.GetTransportState(Transport.POLLING);
                    _curclient.SetContext(Transport.POLLING, context);
                    _curclient.Send(Transport.POLLING, iopacket);
                }
                else
                {
                    _curclient = SocketIO.Instance().GetClient(sid);
                    if (_curclient == null)
                    {
                        SocketIO.CreateResponse(sid, origin).End500(context);
                        return;
                    }
                    if (request.HttpMethod == HttpMethod.POST)
                    {
                        //先返回Post应答包
                        HttpResponse postResponse = SocketIO.CreateResponse(sid, origin);
                        postResponse.ContentType = "text/plain";
                        postResponse.Write(Encoding.UTF8.GetBytes("ok"));
                        postResponse.End(context);
                        IByteStream reqstream = PoolBufferAllocator.Default.AllocStream(request.Body.Length);
                        reqstream.WriteBytes(request.Body);
                        while (reqstream.IsReadable())
                        {
                            SocketIOPacket iopacket = IOPacketParser.Parse(_curclient, reqstream);
                            OnPacket(context, Transport.POLLING, iopacket);
                        }
                    }
                    else if (request.HttpMethod == HttpMethod.GET)
                    {
                        _curclient.SetContext(Transport.POLLING, context);
                        _curclient.ReplySocketIOData(Transport.POLLING);
                        _curclient.Connect();
                    }
                }

            }
            else
            {
                FrameRequest frameReq = msg as FrameRequest;
                if (frameReq != null)
                {
                    switch (frameReq.Frame)
                    {
                        case FrameCodes.Text:
                            {
                                if (_curclient != null)
                                {
                                    IByteStream contentstream = PoolBufferAllocator.Default.AllocStream();
                                    contentstream.WriteBytes(frameReq.Content);
                                    while (contentstream.IsReadable())
                                    {
                                        SocketIOPacket iopacket = IOPacketParser.Parse(_curclient, contentstream);
                                        OnPacket(context, Transport.WEBSOCKET, iopacket);
                                    }
                                }
                            }
                            break;
                        case FrameCodes.Connected:
                            {
                                if (_curclient == null)
                                {
                                    //直接websocket连接
                                    string sessionid = Guid.NewGuid().ToString("N");
                                    _curclient = IOClient.Create(Transport.WEBSOCKET, sessionid);
                                    _curclient.SetContext(Transport.WEBSOCKET, context);
                                    AuthData authdata = new AuthData();
                                    authdata.sid = sessionid;
                                    authdata.upgrades = new string[] { "websocket" };
                                    authdata.pingInterval = SocketIO.Instance().PingInterval;
                                    authdata.pingTimeout = SocketIO.Instance().PingTimeout;
                                    SocketIOPacket iopacket = new SocketIOPacket(PacketType.OPEN);
                                    iopacket.SetData(authdata);
                                    _curclient.Send(iopacket);
                                    _curclient.ResetPingTimeout();
                                    _curclient.Connect();
                                }
                                else
                                {
                                    //升级协议定时过期
                                    _curclient.ResetUpgradeTimeout();
                                }

                            }
                            break;
                    }
                }
                context.FireNextRead(msg);
            }
        }

        public override void ChannelWrite(IContext context, object msg)
        {
            HandshakeResponse websockethandshake = msg as HandshakeResponse;
            if (websockethandshake != null)
            {
                string sid = websockethandshake.Request.Query.Get("sid");
                if (string.IsNullOrEmpty(sid))
                {
                    context.FirePreWrite(msg);
                    return;
                }
                _curclient = SocketIO.Instance().GetClient(sid);
                if (_curclient != null)
                {
                    _curclient.SetContext(Transport.WEBSOCKET, context);
                }
                context.FirePreWrite(msg);
            }
            else
            {
                context.FirePreWrite(msg);
            }

        }


        /// <summary>
        /// 处理接收包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        private void OnPacket(IContext context, Transport transport, SocketIOPacket packet)
        {
            if (packet == null) return;
            switch (packet.GetResponseType())
            {
                case PacketType.PING:
                    {
                        SocketIOPacket outPacket = new SocketIOPacket(PacketType.PONG);
                        outPacket.SetData(packet.GetData());
                        _curclient.Send(transport, outPacket);
                        if ("probe".Equals(packet.GetData()))
                        {
                            _curclient.Send(Transport.POLLING, new SocketIOPacket(PacketType.NOOP));
                        }
                        else
                        {
                            _curclient.ResetPingTimeout();
                        }
                        break;
                    }

                case PacketType.UPGRADE:
                    {
                        //清空数据包
                        _curclient.ResetPingTimeout();
                        _curclient.CancelUpgradeTimeout();
                        _curclient.UpgradeCurrentTransport();
                        _curclient.Connect();
                        break;
                    }
                case PacketType.CLOSE:
                    {
                        _curclient.Disconnect();
                        break;
                    }
                case PacketType.MESSAGE:
                    {
                        SubPacketType spt = packet.GetSubType();
                        if (SubPacketType.CONNECT == spt)
                        {
                            Namespace ns = SocketIO.Instance().NSHub.Get(packet.GetNsp());
                            if (ns != null && !ns.Contain(_curclient.SessionID))
                            {
                                _curclient.ConnectNamespace(ns);
                                _curclient.Send(packet);
                            }
                            else
                            {
                                SocketIOPacket connerrpacket = new SocketIOPacket(PacketType.MESSAGE);
                                connerrpacket.SetSubType(SubPacketType.ERROR);
                                connerrpacket.SetNsp(packet.GetNsp());
                                connerrpacket.SetData("Invalid namespace");
                                _curclient.Send(connerrpacket);
                            }
                        }
                        else
                        {
                            NSClient nsclient = _curclient.GetNSClient(packet.GetNsp());
                            if (nsclient != null)
                            {
                                if (SubPacketType.DISCONNECT == spt)
                                {
                                    nsclient.Disconnect();
                                }
                                else if (SubPacketType.EVENT == spt)
                                {
                                    //触发事件
                                    IOEventArgs ioEvtArgs = new IOEventArgs(packet.GetData(), (object data) =>
                                    {
                                        //创建回复包
                                        SocketIOPacket ack = new SocketIOPacket(PacketType.MESSAGE);
                                        ack.SetName(packet.GetName());
                                        ack.SetSubType(SubPacketType.ACK);
                                        ack.SetAckId(packet.GetAckId());
                                        ack.SetData(data);
                                        ack.SetNsp(packet.GetNsp());
                                        _curclient.Send(ack);
                                    });
                                    nsclient.EmitEvent(packet.GetName(), ioEvtArgs);
                                }
                                else if (SubPacketType.ACK == spt)
                                {
                                    //客户端对服务器的ack
                                    IOEventArgs ioEvtArgs = new IOEventArgs(packet.GetData(), null);
                                    nsclient.EmitEvent(packet.GetName(), packet.GetAckId(), ioEvtArgs);
                                }
                            }

                        }

                        break;
                    }
                default:
                    break;
            }

        }

        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }


    }
}
