using System;

namespace TemplateAction.Core
{
    public class ConcurrentProxy
    {
        private volatile object _target;
        private object _lock = new object();
        internal ConcurrentProxy()
        {
        }
        public object GetValue(PluginCollection collection, ServiceDescriptor desc, ILifetimeFactory extOtherFactory)
        {
            if (_target == null)
            {
                lock (_lock)
                {
                    if (_target == null)
                    {
                        _target = collection.CreateServiceInstance(desc.ServiceType, desc.Factory, extOtherFactory);
                    }
                }
            }
            return _target;
        }
    }
}
