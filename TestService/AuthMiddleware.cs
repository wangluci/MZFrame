using System;
using TemplateAction.Core;

namespace TestService
{
    public class AuthMiddleware : IFilterMiddleware
    {
        public object Excute(TAAction ac, FilterMiddlewareNode next)
        {
            if (ac.NameSpace == "TestService" && ac.Controller == "Home" && ac.Action == "Test")
            {
                //需要身份认证
                ac.Context.Response.Write("带有身份认证\n");
                return next.Excute(ac);
            }
            else
            {
                //不需要认证
                return next.Excute(ac);
            }
        }
    }
}
