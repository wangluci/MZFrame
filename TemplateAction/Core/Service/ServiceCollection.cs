using System;
using System.Collections.Generic;
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
                if (_services.TryGetValue(key, out sd))
                {
                    return sd;
                }
                return null;
            }
        }
        public void Add(string key, ServiceDescriptor des)
        {
            if (des != null)
            {
                des.PluginName = _pluginName;
                _services[key] = des;
            }
        }
       
    }
}
