using System;
using MyNet.Buffer;
using MyNet.Handlers;
using System.Collections.Generic;
using MyNet.Middleware.Http.WebSocket;

namespace MyNet.Middleware.Http
{
    public class WebSocketState : IHttpHandlerState
    {
        protected IStateSource _source;
        protected HttpConfig _config;
        public WebSocketState(IContext context, IStateSource source, HttpConfig config)
        {
            _source = source;
            _config = config;
        }
        public void Inactive(IContext context)
        {
        }

        public void Read(IByteStream stream, IContext context, bool isssl)
        {
            stream.SetReaderIndex(0);
            List<FrameRequest> list = new List<FrameRequest>();
            while (true)
            {
                try
                {
                    FrameRequest request = FrameRequest.Parse(stream);
                    if (request == null)
                    {
                        context.Channel.MergeRead();
                        return;
                    }
                    list.Add(request);
                    if (request.Fin)
                    {
                        break;
                    }
                }
                catch (NotSupportedException ex)
                {
                    Common.AgentLogger.Instance.Err(string.Format("WebSocketState错误：{0}", ex.Message));
                    break;
                }
            }
            context.Channel.FinishRead();
            foreach (FrameRequest request in list)
            {
                context.FireNextRead(request);
            }

        }


        public void Write(IContext context, object msg)
        {
            FrameResponse response = msg as FrameResponse;
            if (response != null)
            {
                HttpResponseEncoder.WriteWebSocketFrameResponse(response, false);
            }
            context.FirePreWrite(msg);
        }


        public void WriteFinish(IContext context, object msg, bool isssl)
        {
        }
    }
}
