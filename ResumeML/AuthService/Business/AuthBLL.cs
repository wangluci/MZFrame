using Common;
using Common.Redis;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// 生成签名
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        public virtual string _MakeClientSign(ClientTokenInfo info, string tk)
        {
            StringBuilder sb = new StringBuilder(200);
            sb.Append(info.Account);
            sb.Append(info.TokenExpire);
            sb.Append(info.TokenStart);
            sb.Append(info.UserId);
            sb.Append(tk);
            return MyAccess.Core.Crypter.SHA1(sb.ToString(), System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 生成客户端令牌
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        public virtual string _MakeClientToken(ClientTokenInfo info, string tk)
        {
            ClientToken clientToken = new ClientToken();
            info.TokenStart = MyAccess.Core.TypeConvert.Time2JavaLong(DateTime.Now);
            info.TokenExpire = MyAccess.Core.TypeConvert.Time2JavaLong(DateTime.Now.AddDays(7));
            clientToken.Info = info;
            clientToken.Sign = _MakeClientSign(info, tk);
            return MyAccess.Core.Crypter.EncodeBase64(MyAccess.Json.Json.Encode(clientToken), System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="terminal">登录终端</param>
        /// <returns></returns>
        public virtual BusResponse<string> Login(string username, string password, string terminal)
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
            ClientTokenInfo tkinfo = new ClientTokenInfo();
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
            tkinfo.Account = account.UserName;
            tkinfo.UserId = account.Id;
            string clienttoken = _MakeClientToken(tkinfo, newtoken);
            if (!string.IsNullOrEmpty(newtoken))
            {
                SysLog log = new SysLog();
                log.UserId = account.Id;
                log.CreateDate = DateTime.Now;
                log.LogType = 1;
                log.IPAddress = terminal;
                log.Info = "登录成功";
                _auth.AddSysLog(log);

                return BusResponse<string>.Success(clienttoken);
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
