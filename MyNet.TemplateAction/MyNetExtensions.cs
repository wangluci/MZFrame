using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;

namespace MyNet.TemplateAction
{
    public static class MyNetExtensions
    {
        /// <summary>
        /// 使用session
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static TAApplication UseSession(this TAApplication app)
        {
            TAEventDispatcher.Instance.Register(new DefaultHandler<ContextCreatedEvent>(evt =>
            {
                HttpContext context = evt.Context as HttpContext;
                if (context != null)
                {
                    context.CreateSession();
                }
            }));
            return app;
        }

    }
}
