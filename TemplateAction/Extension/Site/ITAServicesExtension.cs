
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    /// <summary>
    /// 服务接口扩展，可获取Scope服务
    /// </summary>
    public static class ITAServicesExtension
    {
        public static object GetServiceWithScope(this ITAServices services, string key)
        {
            return services.GetService(key, TAAction.Current);
        }
        public static T GetServiceWithScope<T>(this ITAServices services) where T : class
        {
            return services.GetService<T>(TAAction.Current);
        }
    }
}
