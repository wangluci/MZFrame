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

                Exception proceedEx = null;
                try
                {
                    await proceed(invocation, proceedInfo);
                }
                catch (Exception ex)
                {
                    proceedEx = ex;
                }
                finally
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                       await attribute.ProceedAfter(dbHelp, proceedEx, invocation);
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

                Exception proceedEx = null;
                TResult result = default(TResult);
                try
                {
                    result = await proceed(invocation, proceedInfo);
                }
                catch (Exception ex)
                {
                    proceedEx = ex;
                }
                finally
                {
                    foreach (AbstractAopAttr attribute in Attributes)
                    {
                        await attribute.ProceedAfter(dbHelp, proceedEx, invocation);
                    }
                    //同步
                    //结束后自动清参数
                    dbHelp?.EnableAndClearParam();
                }
                return result;
            }
            else
            {
                return await proceed(invocation, proceedInfo);
            }
        }
    }
}
