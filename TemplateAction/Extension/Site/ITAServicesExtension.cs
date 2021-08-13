using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    /// <summary>
    /// 服务接口扩展，可获取Scope服务
    /// </summary>
    public static class ITAServicesExtension
    {
        public static object GetAllService(this ITAServices services, string key)
        {
            return services.GetService(key, TAAction.Current);
        }
        public static T GetAllService<T>(this ITAServices services) where T : class
        {
            return services.GetService<T>(TAAction.Current);
        }
        public static List<T> GetAllServices<T>(this ITAServices services) where T : class
        {
            return services.GetServices<T>(TAAction.Current);
        }
    }
}
