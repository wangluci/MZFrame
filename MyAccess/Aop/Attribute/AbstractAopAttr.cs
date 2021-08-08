using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using System;

namespace MyAccess.Aop
{
    public abstract class AbstractAopAttr : Attribute
    {
        /// <summary>
        /// 自定义注解拦截器
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns>是否调用了invocation的Proceed函数</returns>
        public abstract bool InterceptDeal(IInvocation invocation);
    }
}
