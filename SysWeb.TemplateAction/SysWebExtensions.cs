using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;

namespace SysWeb.TemplateAction
{
    public static class SysWebExtensions
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
                SysWebContext syswebc = evt.Context as SysWebContext;
                if (syswebc != null)
                {
                    syswebc.CreateSession();
                }
            }));
            return app;
        }

    }
}
