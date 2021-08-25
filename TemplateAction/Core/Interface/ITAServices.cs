
using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateScopeService(Type serviceType, ILifetimeFactory scopeFactory, ProxyFactory factory = null);
        object GetService(string key, ILifetimeFactory scopeFactory = null);
        object GetService(Type tp, ILifetimeFactory scopeFactory = null);
    }
}
