using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 为插件注册的中间件提供热更新
    /// </summary>
    internal class PluginMiddleware : IFilterMiddleware
    {
        private string _key;
        public PluginMiddleware(string key)
        {
            _key = key;
        }
        public object Excute(TAAction ac, FilterMiddlewareNode next)
        {
            IFilterMiddleware filter = ac.Context.Application.ServiceProvider.GetService(_key) as IFilterMiddleware;
            if (filter != null)
            {
                return filter.Excute(ac, next);
            }
            else
            {
                return next.Excute(ac);
            }
        }
    }
}
