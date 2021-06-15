using System;
using System.Collections.Concurrent;
namespace MyNet.SocketIO
{
    /// <summary>
    /// 命名空间管理中心
    /// </summary>
    public class NamespacesHub
    {
        private ConcurrentDictionary<string, Namespace> _namespaces = new ConcurrentDictionary<string, Namespace>();
        public Namespace Create(string name)
        {
            return _namespaces.GetOrAdd(name, x =>
            {
                return new Namespace(x);
            });
        }
 
        public Namespace Get(string name)
        {
            Namespace ns;
            if (_namespaces.TryGetValue(name, out ns))
            {
                return ns;
            }
            else
            {
                return null;
            }
        }
        public Namespace Remove(string name)
        {
            Namespace ns;
            if (_namespaces.TryRemove(name, out ns))
            {
                return ns;
            }
            else
            {
                return null;
            }
        }
    }
}
