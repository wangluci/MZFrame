using Castle.DynamicProxy;
using System.Reflection;

namespace MyAccess.Aop
{
    public class EntitylIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            MethodInfo mi = invocation.MethodInvocationTarget;
            IBaseEntity be = invocation.Proxy as IBaseEntity;
            if (be != null && mi.Name.StartsWith("set_"))
            {
                if(be.EnableRecord())
                {
                    PropertyInfo pi = invocation.TargetType.GetProperty(mi.Name.Substring(4));
                    if (pi != null)
                    {
                        be.AddProperty(pi);
                    }
                }
            }

            invocation.Proceed();
        }
    }
}
