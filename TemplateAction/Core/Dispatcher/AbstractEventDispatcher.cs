using System;
using System.Reflection;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public abstract class AbstractEventDispatcher : IDispatcher, IEventRegister
    {
        private void PriAddListener(Type interfacetype, object listener)
        {
            MethodInfo[] info = interfacetype.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo mi in info)
            {
                string lstrUrl = "lstr://" + interfacetype.Name + "/" + mi.Name;
                Register<object[], DefaultHandler<object[]>>(lstrUrl.ToLower(), new DefaultHandler<object[]>(parameters =>
                {
                    mi.Invoke(listener, parameters);
                }));
            }
        }
        public void AddListener(object listener)
        {
            Type[] interfaces = listener.GetType().GetInterfaces();
            if (interfaces.Length == 0) return;
            Type curinterface = interfaces[0];
            PriAddListener(curinterface, listener);
        }
        /// <summary>
        /// 通过监听器监听事件
        /// </summary>
        /// <param name="interfacename"></param>
        /// <param name="listener"></param>
        public void AddListener(string interfacename, object listener)
        {
            Type curinterface = listener.GetType().GetInterface(interfacename);
            if (curinterface == null)
            {
                AddListener(listener);
            }
            else
            {
                PriAddListener(curinterface, listener);
            }
        }



        public abstract void Dispatch<T>(string key, T evt) where T : class;
        public abstract bool IsExistHandler(string key);
        public abstract void Register<T, X>(string key, X handler)
            where T : class
            where X : ITAEventHandler<T>;
    }
}
