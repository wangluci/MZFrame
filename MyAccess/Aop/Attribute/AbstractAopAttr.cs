using Castle.DynamicProxy;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public abstract class AbstractAopAttr : Attribute
    {
        /// <summary>
        /// 拦截的方法执行前
        /// </summary>
        /// <param name="dbhelp"></param>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public virtual async Task ProceedBefore(IDbHelp dbhelp, IInvocation invocation) { await Task.CompletedTask; }
        /// <summary>
        /// 拦截的方法执行异常
        /// </summary>
        /// <param name="dbhelp"></param>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public virtual async Task ProceedException(IDbHelp dbhelp, IInvocation invocation) { await Task.CompletedTask; }
        /// <summary>
        /// 拦截的方法执行后
        /// </summary>
        /// <param name="dbhelp"></param>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public virtual async Task ProceedAfter(IDbHelp dbhelp, IInvocation invocation) { await Task.CompletedTask; }
    }
}
