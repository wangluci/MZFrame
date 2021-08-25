using System;
using System.Collections.Generic;
namespace TemplateAction.Core
{
    public class ServiceCollection : IServiceCollection
    {
        private Dictionary<string, ServiceDescriptorList> _services;
        private string _pluginName;
        public string PluginName
        {
            get { return _pluginName; }
        }
        public ServiceCollection(string plugin)
        {
            _pluginName = plugin;
            _services = new Dictionary<string, ServiceDescriptorList>();
        }
        public IServiceDescriptorEnumerable this[string key]
        {
            get
            {
                ServiceDescriptorList sd;
                if (_services.TryGetValue(key, out sd))
                {
                    return sd;
                }
                return null;
            }
        }
        public void Add(string key, ServiceDescriptor des)
        {
            if (des == null) return;
            ServiceDescriptorList sd;
            if (_services.TryGetValue(key, out sd))
            {
                sd.Add(des);
            }
            else
            {
                des.PluginName = _pluginName;
                _services.Add(key, ServiceDescriptorList.Create(des));
            }
        }

    }
}
