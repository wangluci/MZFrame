using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public class PluginEventDispatcher : IEventDispatcher
    {
        private Dictionary<string, object> _handlers;
        public PluginEventDispatcher()
        {
            _handlers = new Dictionary<string, object>();
        }
        public void Register<T, X>(string key, X handler) where T : class where X : ITAEventHandler<T>
        {
            _handlers[key] = handler;
        }
        public void Dispatch<T>(string key, T evt) where T : class
        {
            object val;
            if (_handlers.TryGetValue(key, out val))
            {
                ITAEventHandler<T> eh = val as ITAEventHandler<T>;
                if (eh != null)
                {
                    eh.OnEvent(evt);
                }
            }
        }
        public void RegisterLoadAfter(Action<TAApplication> ac)
        {
            Register<TAApplication, DefaultHandler<TAApplication>>(TAEventDispatcher.AFTER_EVENT, new DefaultHandler<TAApplication>(ac));
        }
        public void DispathLoadAfter(TAApplication app)
        {
            Dispatch(TAEventDispatcher.AFTER_EVENT, app);
        }

        public bool IsExistDispatcher(string key)
        {
            return _handlers.ContainsKey(key);
        }
    }
}
