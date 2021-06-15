using System;

namespace TemplateAction.Core
{
    public class ConcurrentProxy
    {
        private volatile object _target;
        private object _lock = new object();
        private PluginCollection _collection;
        internal ConcurrentProxy(PluginCollection collection)
        {
            _collection = collection;
        }
        public object GetValue(ServiceDescriptor desc)
        {
            if (_target == null)
            {
                lock (_lock)
                {
                    if (_target == null)
                    {
                        _target = _collection.CreateServiceInstance(desc.ServiceType, desc.Factory);
                    }
                }
            }
            return _target;
        }
    }
}
