using Common;
using Microsoft.Extensions.Options;
using System;
using TemplateAction.Core;

namespace AuthService
{
    public class AuthMiddleware : IFilterMiddleware
    {
        private AuthBLL _authBLL;
        private IOptions<AuthOption> _conf;
        public AuthMiddleware(AuthBLL auth, IOptions<AuthOption> conf)
        {
            _authBLL = auth;
            _conf = conf;
        }
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
                BusResponse<ClientTokenInfo> response = _authBLL.CheckToken(tk, ac.Context.GetTerminal());
                if (!response.IsSuccess())
                {
                    return response.ToAjaxResult(ac.Context);
                }

                //验证权限
                if (_conf.Value.enable_permission)
                {
                    if (!_authBLL.ExistPermission(response.Data.UserId, ac.ActionNode.AboutModule, ac.ActionNode.AboutAction))
                    {
                        return new AjaxResult(ac.Context, -555, "你的权限不足，请联系管理人员分配权限");
                    }
                }


                ac.Context.Items["AuthToken"] = response.Data;

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
