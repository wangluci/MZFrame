using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 代理服务工厂
    /// </summary>
    /// <param name="constructorArguments"></param>
    /// <returns></returns>
    public delegate object ProxyFactory(object[] constructorArguments);
    /// <summary>
    /// 服务描述信息
    /// </summary>
    public class ServiceDescriptor
    {
        public ServiceLifetime Lifetime { get; }
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ProxyFactory Factory { get; }
        /// <summary>
        /// 所属插件
        /// </summary>
        public string PluginName { get; set; }
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime, ProxyFactory factory)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Factory = factory;
        }
    }
    public enum ServiceLifetime
    {
        /// <summary>
        /// 每次都获取相同一个实例,如果注册在插件上，则实例保存在插件上，插件更新时会跟着更新
        /// </summary>
        Singleton,
        /// <summary>
        /// 每次都获取一个新的实例
        /// </summary>
        Transient
    }
}
