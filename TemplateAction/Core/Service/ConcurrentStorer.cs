using System;
using System.Collections.Concurrent;

namespace TemplateAction.Core
{
    /// <summary>
    /// 多线程服务实例存储器
    /// </summary>
    public class ConcurrentStorer
    {
        private ConcurrentDictionary<string, ConcurrentProxy> _instances;
        private PluginCollection _collection;
        internal ConcurrentStorer(PluginCollection collection)
        {
            _collection = collection;
            _instances = new ConcurrentDictionary<string, ConcurrentProxy>();
        }
        public ConcurrentProxy GetOrAdd(string key)
        {
            return _instances.GetOrAdd(key, (k) =>
            {
                return new ConcurrentProxy(_collection);
            });
        }
    }
}
