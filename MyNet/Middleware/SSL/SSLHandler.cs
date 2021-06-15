using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using System;

namespace MyNet.Middleware.SSL
{
    public class SSLHandler : AbstractChannelHandler
    {
        private int _packetLength = 0;
        private SSLAgentStream _mediationStream;
        private IContext _capturedContext;
        private SSLSettings _settings;
        private IByteStream _latestOutput;
        internal bool IsServer
        {
            get { return this._settings is SSLServerSettings; }
        }
        internal IContext CapturedContext
        {
            get { return _capturedContext; }
        }

        public SSLHandler(SSLSettings settings)
        {
            _settings = settings;
            _mediationStream = new SSLAgentStream(this, _settings);
        }

        public override void ChannelRead(IContext context, object msg)
        {
            IByteStream input = msg as IByteStream;
            if (input != null)
            {

                IByteStream output = _latestOutput;
                if (input.ReaderIndex == 0)
                {
                    output = PoolBufferAllocator.Default.AllocStream();
                    _latestOutput = output;
                }
                while (input.ReaderIndex < input.Length)
                {
                    int readableBytes = input.WriterIndex - input.ReaderIndex;
                    // 使用该信息计算当前SSL记录的长度。
                    if (this._packetLength > 0)
                    {
                        if (readableBytes < this._packetLength)
                        {
                            //数据未完整
                            context.Channel.MergeRead();
                            return;
                        }
                        else
                        {
                            this._packetLength = 0;
                        }
                    }

                    //判断是否为SSL加密包,非加密包则执行下一个handler
                    if (readableBytes < SSLUtils.SSL_RECORD_HEADER_LENGTH)
                    {
                        return;
                    }
                    int encryptedPacketLength = SSLUtils.GetEncryptedPacketLength(input, input.ReaderIndex);
                    if (encryptedPacketLength == -1)
                    {
                        context.FireNextRead(msg);
                        return;
                    }

                    if (encryptedPacketLength > readableBytes)
                    {
                        // 数据未完整
                        this._packetLength = encryptedPacketLength;
                        context.Channel.MergeRead();
                        return;
                    }
                    if (encryptedPacketLength > SSLUtils.MAX_ENCRYPTED_PACKET_LENGTH)
                    {
                        // 太大不处理
                        return;
                    }
                    _mediationStream.SetSource(input);
                    if ((_mediationStream.State & SSLHandlerState.Authenticating) != 0)
                    {
                        if (!_mediationStream.TriggerHandshakeRead())
                        {
                            _mediationStream.ReadFromSslStream(null, encryptedPacketLength);
                        }
                    }
                    else
                    {
                        if (EnsureAuthenticated())
                        {
                            //SSL解包
                            _mediationStream.ReadFromSslStream(output, encryptedPacketLength);
                        }
                    }
                }

                if (output != null && output.Length > 0)
                {
                    try
                    {
                        //处理粘包
                        ReReadPacket(context, output);
                    }
                    catch (Exception ex)
                    {
                        Common.AgentLogger.Instance.Err(ex.Message + ":" + BitConverter.ToString(output.ToArray()));
                        if (_latestOutput != null)
                        {
                            Common.AgentLogger.Instance.Err(BitConverter.ToString(_latestOutput.ToArray()));
                        }

                    }

                }
            }
            else
            {
                context.FireNextRead(msg);
            }
        }
        private void ReReadPacket(IContext context, IByteStream output)
        {
            context.FireNextRead(new SSLUnwrapStream(output));
            int otherbytes = output.Length - output.ReaderIndex;
            if (context.Channel.ToggleNoCute() && otherbytes > 0)
            {
                //处理粘包
                _latestOutput = output.Slice(output.ReaderIndex, otherbytes);
                ReReadPacket(context, _latestOutput);
            }
        }
        public override void ChannelWrite(IContext context, object msg)
        {
            if (EnsureAuthenticated())
            {
                if (!(msg is SSLWritePacket))
                {
                    //判断是否非SSL包，是则进行SSL封包，并将当前包当作
                    WritePacket wp = msg as WritePacket;
                    if (wp != null)
                    {
                        _mediationStream.WriteToSslStream(wp);
                        return;
                    }
                }

            }
            context.FirePreWrite(msg);
        }
        public override void ChannelWriteErr(IContext context, object msg)
        {
            SSLWritePacket sslpacket = msg as SSLWritePacket;
            if (sslpacket != null && sslpacket.Parent != null)
            {
                context.FireNextWriteErr(sslpacket.Parent);
            }
            else
            {
                context.FireNextWriteErr(msg);
            }
        }
        public override void ChannelWriteFinish(IContext context, object msg)
        {
            SSLWritePacket sslpacket = msg as SSLWritePacket;
            if (sslpacket != null && sslpacket.Parent != null)
            {
                context.FireNextWriteFinish(sslpacket.Parent);
            }
            else
            {
                context.FireNextWriteFinish(msg);
            }
        }

        /// <summary>
        /// 验证当前是否是已验证状态
        /// </summary>
        /// <returns></returns>
        bool EnsureAuthenticated()
        {
            try
            {
                SSLHandlerState oldState = _mediationStream.State;
                if ((oldState & SSLHandlerState.AuthenticationStarted) == 0)
                {
                    _mediationStream.State = SSLHandlerState.Authenticating;
                    _mediationStream.BeginAuthenticateAsServer();
                    return false;
                }
                return (oldState & SSLHandlerState.Authenticated) != 0;
            }
            catch
            {
                return false;
            }
        }
        public void SendSSLPacket(SSLWritePacket wp)
        {
            if (_capturedContext != null)
            {
                _capturedContext.Channel.SendAsync(wp);
            }
        }
        public override void ChannelInactive(IContext context)
        {
            this._mediationStream.CloseStream();
            context.FireNextInactive();
        }
        public override void HandlerInstalled(IContext context)
        {
            _capturedContext = context;
        }
        public override void HandlerUninstalled(IContext context) { }
    }

}
