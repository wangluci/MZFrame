using Common;
using System;
using TemplateAction.Core;

namespace AuthService
{
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
        public AjaxResult login(string username, string password)
        {
            BusResponse<LoginData> response = _authBLL.Login(username, password, Context.GetTerminal());
            return response.ToAjaxResult(Context);
        }
        public AjaxResult info(string token = "")
        {
            return Err("dd");
        }
    }
}
