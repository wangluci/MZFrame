
using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateExtOtherService(Type serviceType, ILifetimeFactory extOtherFactory);
        object CreateExtOtherService(Type serviceType, ProxyFactory factory, ILifetimeFactory extOtherFactory);
        object GetService(string key, ILifetimeFactory extOtherFactory = null);
        T GetService<T>(ILifetimeFactory extOtherFactory = null) where T : class;
        List<T> GetServices<T>(ILifetimeFactory extOtherFactory = null) where T : class;
    }
}
