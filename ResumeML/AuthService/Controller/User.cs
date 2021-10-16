using Common;
using System;
using TemplateAction.Core;

namespace AuthService
{
    [Des("用户管理", -2)]
    public class User : TABaseController
    {
        private AuthBLL _authBLL;
        private UserBLL _usrBLL;
        public User(AuthBLL auth, UserBLL usr)
        {
            _authBLL = auth;
            _usrBLL = usr;
        }

        /// <summary>
        /// vue-element-admin用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Des("登录权限")]
        public AjaxResult Login(Input_Login ipt)
        {
            BusResponse<Data_Login> response = _authBLL.Login(ipt, Context.GetTerminal());
            return response.ToAjaxResult(Context);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public AjaxResult Info(string token)
        {
            BusResponse<ClientTokenInfo> response = _authBLL.CheckToken(token, Context.GetTerminal());
            if (!response.IsSuccess())
            {
                return response.ToAjaxResult(Context);
            }

            return _usrBLL.GetUserInfo(response.Data.UserId).ToAjaxResult(Context);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public AjaxResult List(Input_UserList ipt)
        {
            return Err(string.Empty);
        }
    }
}
