using Castle.DynamicProxy;

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
            else
            {
                invocation.Proceed();
            }

        }
    }
}
