using System;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    /// <summary>
    /// 注入扩展Scope类型
    /// </summary>
    public static class ScopeServiceExtension
    {
        public class ScopeLifetimeFactory : ILifetimeFactory
        {
            public object GetValue(PluginCollection collection, Type serviceType, ProxyFactory factory, ILifetimeFactory extFactory)
            {
                return extFactory.GetValue(collection, serviceType, factory, extFactory);
            }
        }
        private static ScopeLifetimeFactory _lifetimeFactory = new ScopeLifetimeFactory();
        public static IServiceCollection AddScope<T1, T2>(this IServiceCollection collection)
        {
            return collection.AddScope<T1, T1>(null);
        }
        public static IServiceCollection AddScope<T1>(this IServiceCollection collection)
        {
            return collection.AddScope<T1>(null);
        }
        public static IServiceCollection AddScope<T1>(this IServiceCollection collection, ProxyFactory factory)
        {
            return collection.AddScope<T1, T1>(factory);
        }
        public static IServiceCollection AddScope<T1, T2>(this IServiceCollection collection, ProxyFactory factory)
        {
            return collection.AddScope(typeof(T1).FullName, typeof(T2), factory);
        }
        public static IServiceCollection AddScope(this IServiceCollection collection, string key, Type serviceType, ProxyFactory factory)
        {
            ServiceDescriptor tmp = new ServiceDescriptor(serviceType, ServiceLifetime.Other, factory, _lifetimeFactory);
            collection.Add(key, tmp);
            return collection;
        }
    }
}
