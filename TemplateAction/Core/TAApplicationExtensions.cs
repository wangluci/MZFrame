using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Core.Extensions
{
    public static class TAApplicationExtensions
    {
        public static TARequestHandleBuilder UseRoute(this TAApplication app, ITAContext context)
        {
            return app.Route(context);
        }
        public static TAApplication UseRouterBuilder(this TAApplication app, IRouterBuilder builder)
        {
            return app.SetRouterBuilder(builder);
        }
        public static List<AnnotationInfo> GetViewAnnList(this TAApplication app)
        {
            return app.ViewAnnList();
        }

        public static ControllerNode GetControllerNode(this TAApplication app, string ns, string controller)
        {
            return app.FindControllerNodeByKey(ns, controller);
        }
        public static bool ControllerExist(this TAApplication app, string controller)
        {
            return app.ContainController(controller);
        }

    }
}
