using Common;
using Common.Redis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService
{
    public class AuthBLL
    {
        private AuthDAL _auth;
        private PermissionDAL _permission;
        private AuthRedisHelper _redis;
        private IOptions<AuthOption> _conf;
        private UserDAL _user;
        public AuthBLL(AuthDAL auth, UserDAL user, PermissionDAL permission, AuthRedisHelper redis, IOptions<AuthOption> conf)
        {
            _auth = auth;
            _user = user;
            _permission = permission;
            _redis = redis;
            _conf = conf;
        }

        private bool _ExistPermission(long uid, string module, string action)
        {
            //判断是否有权限声明
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
            return false;
        }
        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        private string _MakeClientSign(ClientTokenInfo info, string tk)
        {
            StringBuilder sb = new StringBuilder(200);
            sb.Append(info.Account);
            sb.Append(info.Expire);
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
        private string _MakeClientToken(ClientTokenInfo info, string tk)
        {
            ClientToken clientToken = new ClientToken();
            if (_conf.Value.expire_hours > 0)
            {
                info.Expire = MyAccess.Core.TypeConvert.Time2JavaLong(DateTime.Now.AddHours(_conf.Value.expire_hours));
            }
            else
            {
                info.Expire = -1;
            }
            clientToken.Info = info;
            clientToken.Sign = _MakeClientSign(info, tk);
            return MyAccess.Core.Crypter.EncodeBase64(MyAccess.Json.Json.Encode(clientToken), System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 解释token字符串
        /// </summary>
        /// <param name="tk"></param>
        /// <returns></returns>
        private ClientToken _ParseClientToken(string tk)
        {
            ClientToken ct = null;
            try
            {
                string decode = MyAccess.Core.Crypter.DecodeBase64(tk, Encoding.UTF8);
                ct = MyAccess.Json.Json.DecodeType<ClientToken>(decode);
            }
            catch { }
            return ct;
        }

        /// <summary>
        /// 刷新过期时间
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="terminal"></param>
        private void _RefreshTokenExpire(long uid, string terminal)
        {
            string tkey = string.Format("Token{0}_{1}", uid, terminal);
            TimeSpan ts = DateTime.Now.AddHours(_conf.Value.expire_hours) - DateTime.Now;
            _redis.KeyExpire(tkey, ts);
        }
        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        private string _MakeRefreshToken(ClientTokenInfo info, string tk)
        {
            return MyAccess.Core.Crypter.SHA1(info.UserId + tk, System.Text.Encoding.UTF8);
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
            return _ExistPermission(uid, module, action);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="terminal">登录终端</param>
        /// <returns></returns>
        public virtual BusResponse<LoginData> Login(string username, string password, string terminal)
        {
            AdminInfo account = _user.GetAdminByName(username);
            if (account == null) return BusResponse<LoginData>.Error(-11, "用户不存在！");

            string limitmsg = _auth.LimitLoginTime(account.Id);

            if (!string.IsNullOrEmpty(limitmsg))
            {
                return BusResponse<LoginData>.Error(-12, limitmsg);
            }

            password = MyAccess.Core.Crypter.MD5(string.Concat(password, "@TANetAuth"));
            if (!password.Equals(account.Password, StringComparison.OrdinalIgnoreCase))
            {
                SysLog log = new SysLog();
                log.UserId = account.Id;
                log.CreateDate = DateTime.Now;
                log.LogType = 0;
                log.IPAddress = terminal;
                log.Info = "登录密码错误";
                _auth.AddSysLog(log);
                return BusResponse<LoginData>.Error(-13, "登录密码错误！");
            }
            else
            {
                //权限验证是否可登录
                if (_conf.Value.enable_permission)
                {
                    if (!_ExistPermission(account.Id, "/AuthService/User", "login"))
                    {
                        return BusResponse<LoginData>.Error(-15, "该用户被禁止登录！");
                    }
                }
            }

            //新建令牌
            ClientTokenInfo tkinfo = new ClientTokenInfo();
            string intoken = null;
            if (_conf.Value.enable_sso)
            {
                try
                {
                    intoken = Guid.NewGuid().ToString("N");
                    string tkey = string.Format("Token{0}_{1}", account.Id, terminal);
                    TimeSpan ts = DateTime.Now.AddHours(_conf.Value.expire_hours) - DateTime.Now;
                    _redis.StringSet(tkey, intoken, ts);
                }
                catch (Exception ex)
                {
                    return BusResponse<LoginData>.Error(-16, ex.Message);
                }
            }
            else
            {
                intoken = _conf.Value.sign_key;
            }

            tkinfo.Account = account.UserName;
            tkinfo.UserId = account.Id;
            string clienttoken = _MakeClientToken(tkinfo, intoken);
            if (!string.IsNullOrEmpty(intoken))
            {
                SysLog log = new SysLog();
                log.UserId = account.Id;
                log.CreateDate = DateTime.Now;
                log.LogType = 1;
                log.IPAddress = terminal;
                log.Info = "登录成功";
                _auth.AddSysLog(log);

                LoginData lgdata = new LoginData();
                lgdata.expire = tkinfo.Expire;
                lgdata.refresh_token = _MakeRefreshToken(tkinfo, intoken);
                lgdata.token = clienttoken;
                return BusResponse<LoginData>.Success(lgdata);
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
                return BusResponse<LoginData>.Error(-21, "登录令牌生成失败！");
            }
        }
        /// <summary>
        /// 校验令牌
        /// </summary>
        /// <param name="tk"></param>
        /// <returns></returns>
        public virtual BusResponse<ClientTokenInfo> CheckToken(string tk, string terminal)
        {
            ClientToken ct = _ParseClientToken(tk);
            if (ct == null)
            {
                return BusResponse<ClientTokenInfo>.Error(50008, "登录令牌信息错误");
            }

            string tkey = string.Format("Token{0}_{1}", ct.Info.UserId, terminal);
            string intoken = null;
            if (_conf.Value.enable_sso)
            {
                try
                {
                    intoken = _redis.StringGet(tkey);
                }
                catch { }
                if (string.IsNullOrEmpty(intoken))
                {
                    return BusResponse<ClientTokenInfo>.Error(50014, "登录令牌过期");
                }
            }
            else
            {
                intoken = _conf.Value.sign_key;
            }


            string tkstr = _MakeClientSign(ct.Info, intoken);
            if (!tkstr.Equals(ct.Sign))
            {
                return BusResponse<ClientTokenInfo>.Error(50012, "其他客户端登录了");
            }

            if (_conf.Value.expire_hours > 0)
            {
                //判断令牌是否过期
                DateTime exp = MyAccess.Core.TypeConvert.JavaLongTime2CSharp(ct.Info.Expire);
                if (exp < DateTime.Now)
                {
                    return BusResponse<ClientTokenInfo>.Error(50014, "登录令牌过期");
                }
            }

            return BusResponse<ClientTokenInfo>.Success(ct.Info);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="refreshtk"></param>
        /// <param name="tk"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public virtual BusResponse<LoginData> RefreshToken(string refreshtk, string tk, string terminal)
        {
            ClientToken ct = _ParseClientToken(tk);
            if (ct == null)
            {
                return BusResponse<LoginData>.Error(50008, "登录令牌信息错误");
            }
            string tkey = string.Format("Token{0}_{1}", ct.Info.UserId, terminal);
            string intoken = null;
            try
            {
                intoken = _redis.StringGet(tkey);
            }
            catch { }
            if (string.IsNullOrEmpty(intoken))
            {
                return BusResponse<LoginData>.Error(50014, "登录令牌过期");
            }
            string mkrefreshtk = _MakeRefreshToken(ct.Info, intoken);
            if (!mkrefreshtk.Equals(refreshtk))
            {
                return BusResponse<LoginData>.Error(-977, "刷新令牌错误");
            }

            _RefreshTokenExpire(ct.Info.UserId, terminal);

            LoginData lgdata = new LoginData();
            lgdata.refresh_token = _MakeRefreshToken(ct.Info, intoken);
            lgdata.token = _MakeClientToken(ct.Info, intoken);
            lgdata.expire = ct.Info.Expire;
            return BusResponse<LoginData>.Success(lgdata);
        }
    }
}
