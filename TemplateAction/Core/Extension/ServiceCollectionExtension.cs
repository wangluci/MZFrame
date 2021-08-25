using System;
using TemplateAction.Common;
namespace TemplateAction.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddScope(this IServiceCollection collection, Type impType, Type serviceType, ProxyFactory factory = null)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Scope, factory));
            return collection;
        }
        public static IServiceCollection AddScope<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            return collection.AddScope<T1, T1>(factory);
        }
        public static IServiceCollection AddScope<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            return collection.AddScope(typeof(T1).FullName, typeof(T2), factory);
        }
        public static IServiceCollection AddScope(this IServiceCollection collection, string key, Type serviceType, ProxyFactory factory = null)
        {
            ServiceDescriptor tmp = new ServiceDescriptor(serviceType, ServiceLifetime.Scope, factory);
            collection.Add(key, tmp);
            return collection;
        }

        public static IServiceCollection AddTransient<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.AddTransient<T1, T1>(factory);
            return collection;
        }
        public static IServiceCollection AddTransient<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Transient, factory));
            return collection;
        }
        public static IServiceCollection AddTransient(this IServiceCollection collection, Type impType, Type serviceType, ProxyFactory factory = null)
        {
            collection.Add(impType.FullName, new ServiceDescriptor(serviceType, ServiceLifetime.Transient, factory));
            return collection;
        }
        public static IServiceCollection AddSingleton<T1, T2>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.Add(typeof(T1).FullName, new ServiceDescriptor(typeof(T2), ServiceLifetime.Singleton, factory));
            return collection;
        }

        public static IServiceCollection AddSingleton<T1>(this IServiceCollection collection, ProxyFactory factory = null)
        {
            collection.AddSingleton<T1, T1>(factory);
            return collection;
        }
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
