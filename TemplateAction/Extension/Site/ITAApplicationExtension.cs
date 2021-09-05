using System;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    public static class ITAApplicationExtension
    {
        /// <summary>
        /// 按顺序使用指定中间件，需要先AddSingle注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static ITAApplication UseMiddleware<T>(this ITAApplication app) where T : class, IFilterMiddleware
        {
            ((TASiteApplication)app).UseMiddleware<T>();
            return app;
        }
        /// <summary>
        /// 将中间件放在第一个执行位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static ITAApplication UseMiddlewareFirst<T>(this ITAApplication app) where T : class, IFilterMiddleware
        {
            ((TASiteApplication)app).UseMiddlewareFirst<T>();
            return app;
        }
    }
}
