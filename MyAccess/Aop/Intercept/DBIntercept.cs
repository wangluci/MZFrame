using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 数据链路层拦截器
    /// </summary>
    public class DBIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //排公有方法不拦截
            if (!invocation.MethodInvocationTarget.IsPublic)
            {
                invocation.Proceed();
                return;
            }
            DBSupportBase support = invocation.InvocationTarget as DBSupportBase;
            if (support != null)
            {
                support.InitHelp();
                try
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
                finally
                {
                    Attribute attrib = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
                    if (attrib != null)
                    {
                        //异步
                        Task rt = invocation.ReturnValue as Task;
                        rt.ContinueWith((t) =>
                        {
                            IDbHelp help = support.DBHelp;
                            if (help != null)
                            {
                                help.EnableAndClearParam();
                            }

                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    else
                    {
                        //同步
                        //结束后自动清参数
                        IDbHelp help = support.DBHelp;
                        if (help != null)
                        {
                            help.EnableAndClearParam();
                        }
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
