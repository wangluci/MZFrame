using System;
using System.Reflection;
using System.Collections.Generic;
namespace TemplateAction.Core
{
    /// <summary>
    /// 全局事件分发器
    /// </summary>
    public class TAEventDispatcher : AbstractEventDispatcher
    {
        private LinkedList<IDispatcher> _scopelist = new LinkedList<IDispatcher>();
        private Dictionary<string, object> _handlers;
        public const string BEFORE_EVENT = "TA_BEFORE_LOAD";
        public const string AFTER_EVENT = "TA_AFTER_LOAD";
        private class Nested
        {
            // 显式静态构造告诉C＃编译器未标记类型BeforeFieldInit
            // 保证在调用Nested静态类时才进行实例初始化
            static Nested() { }
            internal static readonly TAEventDispatcher Instance = new TAEventDispatcher();
        }
        private TAEventDispatcher()
        {
            _handlers = new Dictionary<string, object>();
        }
        /// <summary>
        /// 事件分发扩展
        /// </summary>
        /// <param name="scope"></param>
        public LinkedListNode<IDispatcher> AddScope(IDispatcher scope)
        {
            return _scopelist.AddLast(scope);
        }
        public bool RemoveScope(IDispatcher scope)
        {
            return _scopelist.Remove(scope);
        }
        public void RemoveScope(LinkedListNode<IDispatcher> scopenode)
        {
            _scopelist.Remove(scopenode);
        }
        public static TAEventDispatcher Instance
        {
            get
            {
                return Nested.Instance;
            }
        }
        public void Register<T, X>(X handler) where T : class where X : ITAEventHandler<T>
        {
            Register<T, X>(typeof(T).ToString(), handler);
        }
        public override void Register<T, X>(string key, X handler)
        {
            _handlers[key] = handler;
        }

        public void Register<T>(string key, Action<T> ac) where T : class
        {
            Register<T, DefaultHandler<T>>(key, new DefaultHandler<T>(ac));
        }
        public void Register<T>(DefaultHandler<T> handler) where T : class
        {
            Register<T, DefaultHandler<T>>(typeof(T).ToString(), handler);
        }
        public void Register<T>(DefaultMultiHandler<T> handler) where T : class
        {
            Register<T, DefaultMultiHandler<T>>(typeof(T).ToString(), handler);
        }
        public override void RegisterLoadAfter<T>(Action<T> ac)
        {
            Register<T, DefaultHandler<T>>(AFTER_EVENT, new DefaultHandler<T>(ac));
        }
        public void RegisterLoadBefore<T>(Action<T> ac) where T : TAApplication
        {
            Register<T, DefaultHandler<T>>(BEFORE_EVENT, new DefaultHandler<T>(ac));
        }
        public override void DispathLoadAfter<T>(T app)
        {
            Dispatch(AFTER_EVENT, app);
        }
        public void DispathLoadBefore<T>(T app) where T: TAApplication
        {
            Dispatch(BEFORE_EVENT, app);
        }
        public void Dispatch<T>(T evt) where T : class
        {
            Dispatch(typeof(T).ToString(), evt);
        }

        public override void Dispatch<T>(string key, T evt)
        {
            object rt;
            if (_handlers.TryGetValue(key, out rt))
            {
                ITAEventHandler<T> eh = rt as ITAEventHandler<T>;
                if (eh != null)
                {
                    eh.OnEvent(evt);
                }
            }
            foreach (IDispatcher di in _scopelist)
            {
                di.Dispatch(key, evt);
            }
        }
        public bool IsExistHandler<T>() where T : class
        {
            return IsExistHandler(typeof(T).ToString());
        }
        public override bool IsExistHandler(string key)
        {
            if (_handlers.ContainsKey(key))
            {
                return true;
            }
            foreach (IDispatcher di in _scopelist)
            {
                if (di.IsExistHandler(key))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
