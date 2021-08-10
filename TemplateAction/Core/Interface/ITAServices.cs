
using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object CreateExtOtherService(Type serviceType, ILifetimeFactory extOtherFactory);
        object GetService(string key, ILifetimeFactory extOtherFactory = null);
        T GetService<T>() where T : class;
        List<T> GetServices<T>() where T : class;
    }
}
