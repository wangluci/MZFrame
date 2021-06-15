
using System;

namespace TemplateAction.Core
{
    public interface IServiceCollection
    {
        ServiceDescriptor this[string key] { get; }
        void AddTransient<T1, T2>();
        void AddTransient<T1, T2>(ProxyFactory factory);
        void AddThread<T1, T2>();
        void AddThread<T1, T2>(ProxyFactory factory);
        void AddSingleton<T1>();
        void AddSingleton<T1, T2>();
        void AddSingleton<T1, T2>(ProxyFactory factory);
    }
}
