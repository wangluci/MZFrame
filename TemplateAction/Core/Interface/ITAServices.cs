
using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateExtOtherService(Type serviceType, LifetimeFactory extOtherFactory);
        object GetService(string key, LifetimeFactory extOtherFactory = null);
        T GetService<T>() where T : class;
        List<T> GetServices<T>() where T : class;
    }
}
