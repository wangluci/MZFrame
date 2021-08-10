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
        public object GetValue(Func<Type, ProxyFactory, LifetimeFactory, object> fun, ServiceDescriptor desc, LifetimeFactory extOtherFactory)
        {
            if (_target == null)
            {
                lock (_lock)
                {
                    if (_target == null)
                    {
                        if (fun != null)
                        {
                            _target = fun.Invoke(desc.ServiceType, desc.Factory, extOtherFactory);
                        }
                    }
                }
            }
            return _target;
        }
    }
}
