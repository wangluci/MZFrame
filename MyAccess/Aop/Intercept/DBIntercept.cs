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
            //判断是否为异步
            Attribute attrib = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
            bool isasync = false;
            if (attrib != null)
            {
                isasync = true;
            }

            DBSupportBase support = invocation.InvocationTarget as DBSupportBase;
            if (support != null)
            {
                support.InitHelp();

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

                if (isasync)
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

                    }).ConfigureAwait(false);
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
            else
            {
                invocation.Proceed();
            }


        }
    }
}
