
using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateExtOtherService(Type serviceType, ILifetimeFactory extOtherFactory, ProxyFactory factory = null);
        object GetService(string key, ILifetimeFactory extOtherFactory = null);
        object GetService(Type tp, ILifetimeFactory extOtherFactory = null);
        List<T> GetServices<T>(ILifetimeFactory extOtherFactory = null) where T : class;
    }
}
