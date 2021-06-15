using MyNet.Buffer;
using MyNet.Channel;
using System;
using System.IO;
using System.Net.Security;

namespace MyNet.Middleware.SSL
{

    /// <summary>
    /// SSL代理验证、封包、解包
    /// </summary>
    public class SSLAgentStream : Stream
    {
        IByteStream _readStream;
        SslStream _sslstream;
        SSLSettings _setting;
        SSLAsyncResult<int> _AsyncRead;
        SSLAsyncResult<int> _AsyncWrite;
        SSLHandlerState _state = SSLHandlerState.UnAuthentication;
        WritePacket _curwritePacket = null;
        bool _needRead = false;
        SSLHandler _owner;
        public SSLHandlerState State
        {
            get { return _state; }
            set { _state = value; }
        }
        public bool NeedRead
        {
            get { return _needRead; }
            set { _needRead = value; }
        }

        public SSLAgentStream(SSLHandler owner, SSLSettings settings)
        {
            _setting = settings;
            _owner = owner;
            _sslstream = new SslStream(this, false);
        }
        public void CloseStream()
        {
            _sslstream.Dispose();
        }
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
            if (disposing)
            {
                if (_AsyncWrite != null)
                {
                    _AsyncWrite = null;
                }
                if (_readStream != null)
                {
                    _readStream = null;
                }
                _needRead = false;
                _state = SSLHandlerState.UnAuthentication;
            }
        }
        /// <summary>
        /// 握手异步回调结果处理
        /// </summary>
        /// <param name="ar"></param>
        private void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                this._sslstream.EndAuthenticateAsServer(ar);
            }
            catch (IOException)
            {
                this.State = SSLHandlerState.UnAuthentication;
                return;
            }
            catch (Exception)
            {
                this.State = SSLHandlerState.UnAuthentication;
                return;
            }
            this.State = SSLHandlerState.Authenticated;
        }
        public void BeginAuthenticateAsServer()
        {
            _needRead = true;
            if (_owner.IsServer)
            {
                SSLServerSettings serverSettings = (SSLServerSettings)this._setting;
                _sslstream.BeginAuthenticateAsServer(serverSettings.Certificate, serverSettings.NegotiateClientCertificate, _setting.EnabledProtocols, serverSettings.CheckCertificateRevocation, AsyncCallback, null);
            }
            else
            {
                SSLClientSettings clientSettings = (SSLClientSettings)this._setting;
                _sslstream.BeginAuthenticateAsClient(clientSettings.TargetHost, clientSettings.X509CertificateCollection, clientSettings.EnabledProtocols, clientSettings.CheckCertificateRevocation, AsyncCallback, null);
            }
        }

        /// <summary>
        /// 握手阶段触发读取数据
        /// </summary>
        public bool TriggerHandshakeRead()
        {
            if (_needRead)
            {
                _needRead = false;
                if (_AsyncWrite != null)
                {
                    _AsyncWrite.Finish();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetSource(IByteStream readstream)
        {
            this._readStream = readstream;
        }
        public void WriteToSslStream(WritePacket packet)
        {
            _curwritePacket = packet;
            ArraySegment<byte> tmparr = packet.Stream.BufferChunk;
            this._sslstream.Write(tmparr.Array, tmparr.Offset, packet.Stream.Length);
        }
        public void ReadFromSslStream(IByteStream outstream, int readcount)
        {
            if (_AsyncRead != null)
            {
                ArraySegment<byte> sslBuffer = this._AsyncRead.SSLAsyncBuffer;
                this._AsyncRead.Result = this.ReadFromInput(sslBuffer.Array, sslBuffer.Offset, sslBuffer.Count);
                this._AsyncRead.Finish();
            }
            else
            {
                byte[] tmpreads = new byte[readcount];
                int readsize = this._sslstream.Read(tmpreads, 0, readcount);
                if (readsize > 0 && outstream != null)
                {
                    outstream.WriteBytes(tmpreads, 0, readsize);
                }
            }
        }
        IAsyncResult PrepareSyncReadResult(int readBytes, object state)
        {
            SSLSyncResult<int> result = new SSLSyncResult<int>();
            result.Result = readBytes;
            result.AsyncState = state;
            return result;
        }
        int ReadFromInput(byte[] destination, int destinationOffset, int destinationCapacity)
        {
            return this._readStream.Read2Array(destination, destinationOffset, destinationCapacity);
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (_readStream != null)
            {
                int canreadbytes = this._readStream.Length - this._readStream.ReaderIndex;
                if (canreadbytes > 0)
                {
                    //有数据则直接读取
                    int read = this.ReadFromInput(buffer, offset, count);
                    return this.PrepareSyncReadResult(read, state);
                }
            }

            _AsyncRead = new SSLAsyncResult<int>();
            _AsyncRead.SSLAsyncBuffer = new ArraySegment<byte>(buffer, offset, count);
            _AsyncRead.Callback = callback;
            _AsyncRead.AsyncState = state;
            return _AsyncRead;
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            SSLResult<int> result = asyncResult as SSLResult<int>;
            if (_AsyncRead != null)
            {
                _AsyncRead.SSLAsyncBuffer = default(ArraySegment<byte>);
                _AsyncRead.Callback = null;
                _AsyncRead = null;
            }
            return result.Result;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.ReadFromInput(buffer, offset, count);
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            _curwritePacket = null;
            Write(buffer, offset, count);
            if ((_state & SSLHandlerState.Authenticating) != 0 && _needRead)
            {
                //如果是在握手阶段则使用异步,需要读取才结束
                _AsyncWrite = new SSLAsyncResult<int>();
                _AsyncWrite.Callback = callback;
                _AsyncWrite.AsyncState = state;
                return _AsyncWrite;
            }
            else
            {
                SSLSyncResult<int> result = new SSLSyncResult<int>();
                result.AsyncState = state;
                return result;
            }
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            return;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            SSLWritePacket wp = new SSLWritePacket(_curwritePacket);
            wp.Stream.WriteBytes(buffer, offset, count);
            _owner.SendSSLPacket(wp);
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }


    }
}
