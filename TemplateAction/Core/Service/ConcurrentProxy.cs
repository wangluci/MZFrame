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
        public object GetValue(Func<Type, ProxyFactory, ILifetimeFactory, object> fun, ServiceDescriptor desc, ILifetimeFactory extOtherFactory)
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
