using Common;
using System;
using TemplateAction.Core;

namespace AuthService
{
    [Des("系统权限")]
    public class User : TABaseController
    {
        private AuthBLL _authBLL;
        public User(AuthBLL auth)
        {
            _authBLL = auth;
        }
       
        /// <summary>
        /// vue-element-admin用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Des("登录权限")]
        public AjaxResult login(string username, string password)
        {
            BusResponse<LoginData> response = _authBLL.Login(username, password, Context.GetTerminal());
            return response.ToAjaxResult(Context);
        }
        public AjaxResult info(string token)
        {
            return Err("dd");
        }
    }
}
