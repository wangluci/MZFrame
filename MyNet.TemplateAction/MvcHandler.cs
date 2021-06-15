using MyNet.Channel;
using MyNet.Common;
using MyNet.Handlers;
using MyNet.Middleware.Http;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MvcHandler : AbstractChannelHandler
    {
        public MvcHandler() { }

        public override void ChannelRead(IContext context, object msg)
        {
            HttpRequest req = msg as HttpRequest;
            if (req != null)
            {
                TAEventDispatcher.Instance.Dispatch(new ActionEvent(context, req));
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
