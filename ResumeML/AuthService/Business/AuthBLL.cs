using Common;
using Common.Redis;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class AuthBLL
    {
        private AuthDAL _auth;
        private PermissionDAL _permission;
        private RedisHelper _redis;
        public AuthBLL(AuthDAL auth, PermissionDAL permission, RedisHelper redis)
        {
            _auth = auth;
            _permission = permission;
            _redis = redis;
        }
        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual bool ExistPermission(long uid, string module, string action)
        {
            //判断是否有权限声明
            if (!string.IsNullOrEmpty(action))
            {
                string comparecode = string.Format("{0}/{1}", module, action);

                List<UserPermission> nurlist = _permission.GetUserPermissionByCode(uid, comparecode);
                if (nurlist.Count > 0)
                {
                    bool hasright = true;
                    foreach (UserPermission nur in nurlist)
                    {
                        if (nur.RightType == 1)
                        {
                            hasright = false;
                            break;
                        }
                    }
                    return hasright;
                }
                else
                {
                    if (_permission.HasRolePermissionByCode(uid, comparecode))
                    {
                        return true;
                    }
                }
            }
            else
            {
                //进行用户权限的判断
                bool haspower = _permission.HasUserPermission(uid, module);
                if (haspower)
                {
                    return true;
                }
                //进行角色权限的判断
                if (_permission.HasRolePermission(uid, module))
                {
                    return true;
                }
            }
            return false;
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
            AdminInfo account = _auth.GetAdminByName(username);
            if (account == null) return BusResponse<string>.Error(-11, "用户不存在！");

            string limitmsg = _auth.LimitLoginTime(account.Id);

            if (!string.IsNullOrEmpty(limitmsg))
            {
                return BusResponse<string>.Error(-12, limitmsg);
            }

            password = MyAccess.Core.Crypter.MD5(string.Concat(password.Trim(), "@TANetAuth"));
            if (!password.Equals(account.Password, StringComparison.OrdinalIgnoreCase))
            {
                SysLog log = new SysLog();
                log.UserId = account.Id;
                log.CreateDate = DateTime.Now;
                log.LogType = 0;
                log.IPAddress = terminal;
                log.Info = "登录密码错误";
                _auth.AddSysLog(log);
                return BusResponse<string>.Error(-13, "登录密码错误！");
            }
            else
            {
                if (!ExistPermission(account.Id, "/AuthService/User", "login"))
                {
                    return BusResponse<string>.Error(-15, "该用户被禁止登录！");
                }
            }

            //新建令牌
            LoginTokenInfo tkinfo = new LoginTokenInfo();
            string newtoken = Guid.NewGuid().ToString("N");
            try
            {
                string tkey = string.Format("Token{0}_{1}", account.Id, terminal);
                TimeSpan ts = DateTime.Now.AddDays(7) - DateTime.Now;
                _redis.StringSet(tkey, newtoken, ts);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(-16, ex.Message);
            }
            usr.AccountName = account.LoginID;
            usr.UserId = account.UserID;
            usr.UserType = LoginType.Person;
            string newtoken = _MakeClientToken(usr, newtoken);
            if (!string.IsNullOrEmpty(newtoken))
            {
                _adminDAl.LogSys(account.Id, 1, "登录成功", context.Request.ClientIP);
                ITACookie tokencookie = context.Request.CreateCookie("AdminToken");
                tokencookie.SetExpires(DateTime.Now.AddDays(1));
                tokencookie.SetValue(newtoken);
                context.Response.SaveCookie(tokencookie);
                return DBReturn<string>.Success();
            }
            else
            {
                SysLog log = new SysLog();
                log.UserId = account.Id;
                log.CreateDate = DateTime.Now;
                log.LogType = 0;
                log.IPAddress = terminal;
                log.Info = "登录令牌生成失败";
                _auth.AddSysLog(log);
                return BusResponse<string>.Error(-21, "登录令牌生成失败！");
            }
        }
    }
}
