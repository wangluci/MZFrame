using System;
using System.Reflection;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public class PluginEventDispatcher : AbstractEventDispatcher
    {
        private Dictionary<string, object> _handlers;
        public PluginEventDispatcher()
        {
            _handlers = new Dictionary<string, object>();
        }
        public override void Register<T, X>(string key, X handler)
        {
            _handlers[key] = handler;
        }
        public override void Dispatch<T>(string key, T evt)
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

        public override bool IsExistHandler(string key)
        {
            return _handlers.ContainsKey(key);
        }

     
    }
}
