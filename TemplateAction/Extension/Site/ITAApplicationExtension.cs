using System;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    public static class ITAApplicationExtension
    {
        public static ITAApplication UseFilterMiddleware<T>(this ITAApplication app) where T : class, IFilterMiddleware
        {
            ((TASiteApplication)app).UseFilterMiddleware<T>();
            return app;
        }
    }
}
