﻿using Castle.DynamicProxy;
using System;
namespace MyAccess.Aop
{
    /// <summary>
    /// 业务层拦截器
    /// </summary>
    public class BLLIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            object[] Attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true);
            bool hasProceed = false;

            foreach (object attribute in Attributes)
            {
                AbstractAopAttr dbtrans = attribute as AbstractAopAttr;
                if (dbtrans != null)
                {
                    if (dbtrans.InterceptDeal(invocation))
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
