using System;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSingleton(this IServiceCollection collection, Type impType, Type serviceType, ProxyFactory factory = null)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Singleton, factory));
            return collection;
        }
        /// <summary>
        /// 注册字符串依赖
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IServiceCollection AddString(this IServiceCollection collection, string name, string val)
        {
            Type strType = typeof(string);
            collection.Add(TAUtility.TypeName2ServiceKey(strType, name), new ServiceDescriptor(strType, ServiceLifetime.Singleton, (object[] arguments) =>
            {
                return val;
            }));
            return collection;
        }
    }
}
