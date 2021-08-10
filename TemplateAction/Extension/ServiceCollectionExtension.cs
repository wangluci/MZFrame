using System;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Extension
{
    public static class ServiceCollectionExtension
    {
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
            collection.Add(TAUtility.TypeName2ServiceKey(strType, name), new ServiceDescriptor(strType, ServiceLifetime.Other, (object[] arguments) =>
            {
                return val;
            }, StringLifetimeFactory));
            return collection;
        }
        public static object StringLifetimeFactory(PluginCollection collection, ServiceDescriptor sd, LifetimeFactory extFactory)
        {
            return collection.CreateServiceInstance(sd.ServiceType, sd.Factory, extFactory);
        }
    }
}
