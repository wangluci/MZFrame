using System;
using TemplateAction.Core;

namespace AuthService
{
    public class AuthMiddleware : IFilterMiddleware
    {
        public object Excute(TAAction ac, FilterMiddlewareNode next)
        {
            if (ac.NameSpace != "AuthService")
            {
                //需要身份认证
                string tk = ac.Context.Request.Header["X-Token"];
                if (string.IsNullOrEmpty(tk))
                {
                    return new AjaxResult(ac.Context, -999, "您目前是在登出状态");
                }
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
