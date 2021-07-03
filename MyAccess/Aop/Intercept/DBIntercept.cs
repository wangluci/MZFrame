using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace MyAccess.Aop
{
    /// <summary>
    /// 数据链路层拦截器
    /// </summary>
    public class DBIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            DBSupport support = invocation.InvocationTarget as DBSupport;
            if (support != null)
            {
                support.InitHelp();
                try
                {
                    object[] Attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true);
                    bool hasProceed = false;

                    foreach (object attribute in Attributes)
                    {
                        DBAbstractAttr dbtrans = attribute as DBAbstractAttr;
                        if (dbtrans != null)
                        {
                            if (dbtrans.InterceptDeal(support, invocation))
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
