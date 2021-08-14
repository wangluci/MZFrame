using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using System;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public abstract class AbstractAopAttr : Attribute
    {
        public virtual async Task ProceedBefore(object state, IInvocation invocation) { await Task.CompletedTask; }
        public virtual async Task ProceedAfter(object state, Exception ex, IInvocation invocation) { await Task.CompletedTask; }
    }
}
