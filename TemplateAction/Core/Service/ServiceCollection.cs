using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class ServiceCollection : IServiceCollection
    {
        private Dictionary<string, ServiceDescriptor> _services;
        private string _pluginName;
        public string PluginName
        {
            get { return _pluginName; }
        }
        public ServiceCollection(string plugin)
        {
            _pluginName = plugin;
            _services = new Dictionary<string, ServiceDescriptor>();
        }
        public ServiceDescriptor this[string key]
        {
            get
            {
                ServiceDescriptor sd;
                if(_services.TryGetValue(key,out sd))
                {
                    return sd;
                }
                return null;
            }
        }

        public void AddTransient<T1, T2>()
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Transient, null);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }
        public void AddTransient<T1, T2>(ProxyFactory factory)
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Transient, factory);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }
        public void AddThread<T1, T2>()
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Thread, null);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }
        public void AddSingleton<T1, T2>()
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Singleton, null);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }

        public void AddSingleton<T1>()
        {
            AddSingleton<T1, T1>();
        }
        public void AddThread<T1, T2>(ProxyFactory factory)
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Thread, factory);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }

        public void AddSingleton<T1, T2>(ProxyFactory factory)
        {
            Type imp = typeof(T1);
            ServiceDescriptor tmp = new ServiceDescriptor(typeof(T2), imp, ServiceLifetime.Singleton, factory);
            tmp.PluginName = PluginName;
            _services[imp.FullName] = tmp;
        }
    }
}
