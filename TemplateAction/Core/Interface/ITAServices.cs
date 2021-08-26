using System;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateScopeService(ILifetimeFactory scopeFactory, Type serviceType);
        object CreateScopeService(ILifetimeFactory scopeFactory, ProxyFactory factory);
        object GetService(string key, ILifetimeFactory scopeFactory = null);
        object GetService(Type tp, ILifetimeFactory scopeFactory = null);
    }
}
