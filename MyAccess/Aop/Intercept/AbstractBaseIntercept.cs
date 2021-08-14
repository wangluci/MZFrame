using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyAccess.Aop.Intercept
{
    public abstract class AbstractBaseIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //判断是否为异步
            Attribute attrib = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
            bool isasync = false;
            if (attrib != null)
            {
                invocation.ReturnValue = InterceptAsync(invocation);
            }
            else
            {
                InterceptSync(invocation);
            }
        }
        /// <summary>
        /// 同步拦截
        /// </summary>
        /// <param name="invocation"></param>
        protected abstract void InterceptSync(IInvocation invocation);
        /// <summary>
        /// 异步拦截
        /// </summary>
        /// <param name="invocation"></param>
        protected abstract Task InterceptAsync(IInvocation invocation);
    }
}
