using MyNet.Channel;
using MyNet.Handlers;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using MyNet.Middleware.Http.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNet.Middleware.Http
{
    public class DefaultWebSocketHandler : AbstractChannelHandler
    {
        private Queue<ITriggerRunnable> _timeout = new Queue<ITriggerRunnable>();
        public DefaultWebSocketHandler(){ }
        private void ClearTimeout()
        {
            if (_timeout.Count > 0)
            {
                _timeout.Dequeue().Cancel();
            }
        }
        public override void ChannelRead(IContext context, object msg)
        {
            ChannelBase channel = context.Channel;
            FrameRequest request = msg as FrameRequest;
            if (request != null)
            {
                switch (request.Frame)
                {
                    case FrameCodes.Connected:
                        context.FireNextRead(msg);
                        break;
                    case FrameCodes.Close:
                        context.FireNextRead(msg);
                        channel.Dispose();
                        break;
                    case FrameCodes.Ping:
                        FrameResponse pong = new FrameResponse(FrameCodes.Pong, request.Content);
                        channel.SendAsync(pong);
                        break;
                    case FrameCodes.Pong:
                        ClearTimeout();
                        break;
                    default:
                        context.FireNextRead(msg);
                        break;
                }
            }
            else
            {
                context.FireNextRead(msg);
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
