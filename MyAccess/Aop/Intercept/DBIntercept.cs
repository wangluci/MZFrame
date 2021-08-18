using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 数据链路层拦截器
    /// </summary>
    public class DBIntercept : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            DBSupport support = invocation.InvocationTarget as DBSupport;
            if (support != null)
            {
                IDbHelp dbHelp = support.CreateHelp();
                AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
                foreach (AbstractAopAttr attribute in Attributes)
                {
                    await attribute.ProceedBefore(dbHelp, invocation);
                }
                try
                {
                    await proceed(invocation, proceedInfo);
                }
                catch
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                        await attribute.ProceedException(dbHelp, invocation);
                    }
                    throw;
                }
                finally
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                       await attribute.ProceedAfter(dbHelp, invocation);
                    }
                    //同步
                    //结束后自动清参数
                    dbHelp?.EnableAndClearParam();
                }
            }
            else
            {
                await proceed(invocation, proceedInfo);
            }
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            DBSupport support = invocation.InvocationTarget as DBSupport;
            if (support != null)
            {
                IDbHelp dbHelp = support.CreateHelp();
                AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
                foreach (AbstractAopAttr attribute in Attributes)
                {
                    await attribute.ProceedBefore(dbHelp, invocation);
                }


                try
                {
                    TResult rt = await proceed(invocation, proceedInfo);
                    invocation.ReturnValue = rt;
                    return rt;
                }
                catch
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                        await attribute.ProceedException(dbHelp, invocation);
                    }
                    throw;
                }
                finally
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                        await attribute.ProceedAfter(dbHelp, invocation);
                    }
                    //同步
                    //结束后自动清参数
                    dbHelp?.EnableAndClearParam();
                }
            }
            else
            {
                return await proceed(invocation, proceedInfo);
            }
        }
    }
}
