using Common;
using System;

namespace AuthService
{
    public class AuthBLL
    {
        private AuthDAL _dal;
        public AuthBLL(AuthDAL dal)
        {
            _dal = dal;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="terminal">登录终端</param>
        /// <returns></returns>
        public BusResponse<string> Login(string username, string password, string terminal)
        {
            AdminInfo account = _dal.GetAdminByName(username);
            if (account == null) return BusResponse<string>.Error(-11, "用户不存在！");

            string limitmsg = _dal.LimitLoginTime(account.Id);
            return null;
        }
    }
}
