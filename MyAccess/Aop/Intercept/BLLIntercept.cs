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
            TransAttribute dbtrans = null;
            object[] Attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true);
            foreach (object attribute in Attributes)
            {
                dbtrans = attribute as TransAttribute;
                if (dbtrans != null) break;
            }
            if (dbtrans != null)
            {
                //判断方法是否为异步
                Type attType = typeof(AsyncStateMachineAttribute);
                Attribute attrib = invocation.Method.GetCustomAttribute(attType);
                if (attrib != null)
                {
                    //异步
                    try
                    {
                        AsyncDBMan.Instance().BeginTrans();
                        invocation.Proceed();
                        ITransReturn tr = invocation.ReturnValue as ITransReturn;
                        if (tr != null)
                        {
                            if (tr.IsSuccess())
                            {
                                AsyncDBMan.Instance().Commit();
                            }
                        }
                        else
                        {
                            AsyncDBMan.Instance().Commit();
                        }
                    }
                    finally
                    {
                        AsyncDBMan.Instance().RollBack();
                    }
                }
                else
                {
                    //同步
                    try
                    {
                        DBMan.Instance().BeginTrans();
                        invocation.Proceed();
                        ITransReturn tr = invocation.ReturnValue as ITransReturn;
                        if (tr != null)
                        {
                            if (tr.IsSuccess())
                            {
                                DBMan.Instance().Commit();
                            }
                        }
                        else
                        {
                            DBMan.Instance().Commit();
                        }
                    }
                    finally
                    {
                        DBMan.Instance().RollBack();
                    }
                }
              
            }
            else
            {
                invocation.Proceed();
            }

        }
    }
}
