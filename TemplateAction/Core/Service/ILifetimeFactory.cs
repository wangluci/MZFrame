using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 生命期处理工厂
    /// </summary>
    public interface ILifetimeFactory
    {
        object GetValue(PluginCollection collection, ServiceDescriptor sd, ILifetimeFactory extFactory);
    }
}
