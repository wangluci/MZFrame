using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 业务层拦截器
    /// </summary>
    public class BLLIntercept : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
            foreach (AbstractAopAttr attribute in Attributes)
            {
                await attribute.ProceedBefore(null, invocation);
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
                    await attribute.ProceedAfter(null, proceedEx, invocation);
                }
            }
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
            foreach (AbstractAopAttr attribute in Attributes)
            {
                await attribute.ProceedBefore(null, invocation);
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
                    await attribute.ProceedAfter(null, proceedEx, invocation);
                }
            }
            return result;
        }


    }
}
