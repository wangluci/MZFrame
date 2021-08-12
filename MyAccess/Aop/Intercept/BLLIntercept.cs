using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyAccess.Aop
{
    /// <summary>
    /// 业务层拦截器
    /// </summary>
    public class BLLIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //判断是否为异步
            Attribute attrib = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
            bool isasync = false;
            if (attrib != null)
            {
                isasync = true;
            }

            object[] Attributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
            bool hasProceed = false;

            foreach (object attribute in Attributes)
            {
                AbstractAopAttr dbtrans = (AbstractAopAttr)attribute;
                if (dbtrans != null)
                {
                    if (dbtrans.InterceptDeal(isasync, invocation))
                    {
                        hasProceed = true;
                    }
                }
            }

            if (!hasProceed)
            {
                invocation.Proceed();
            }

        }
    }
}
