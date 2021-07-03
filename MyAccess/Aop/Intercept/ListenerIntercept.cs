using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace MyAccess.Aop
{
    /// <summary>
    /// 事件分发器用拦截器
    /// </summary>
    public class ListenerIntercept : IInterceptor
    {
        private Action<string, object[]> _call;
        private string _name;
        public ListenerIntercept(Action<string, object[]> callFun)
        {
            _call = callFun;
        }
        public void Intercept(IInvocation invocation)
        {
            if (string.IsNullOrEmpty(_name))
            {
                string proxyname = invocation.Proxy.GetType().Name;
                _name = proxyname.Substring(0, proxyname.Length - 5);
            }

            string lstrUrl = "lstr://" + _name + "/" + invocation.Method.Name;
            _call(lstrUrl.ToLower(), invocation.Arguments);
        }
    }
}
