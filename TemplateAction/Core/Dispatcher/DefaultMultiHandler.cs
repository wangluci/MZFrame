using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 多个事件处理器
    /// </summary>
    public class DefaultMultiHandler<T> : ITAEventHandler<T> where T : class
    {
        private LinkedList<Action<T>> _handlers;
        public DefaultMultiHandler()
        {
            _handlers = new LinkedList<Action<T>>();
        }
        public LinkedListNode<Action<T>> Register(Action<T> ac)
        {
            return _handlers.AddLast(ac);
        }
        public bool UnRegister(Action<T> ac)
        {
            return _handlers.Remove(ac);
        }
        public void UnRegister(LinkedListNode<Action<T>> node)
        {
            _handlers.Remove(node);
        }
        public void OnEvent(T evt)
        {
            foreach (Action<T> ac in _handlers)
            {
                ac(evt);
            }
        }
    }
}
