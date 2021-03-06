using System;

namespace AuthService
{
    public class AuthOption
    {
        public string[] auth_modules { get; set; }
        public string connstr { get; set; }
        public string redisconn { get; set; }
        /// <summary>
        /// 是否启用权限控制
        /// </summary>
        public bool enable_permission { get; set; }
        /// <summary>
        /// 权限列表来源，0为插件自动生成，1为redis来源，2为数据库来源
        /// </summary>
        public int permission_from { get; set; }
        /// <summary>
        /// 是否启用单点登录,false则要设置sign_key,为true不用设置sign_key
        /// </summary>
        public bool enable_sso { get; set; }
        /// <summary>
        /// 签名密钥
        /// </summary>
        public string sign_key { get; set; }
        /// <summary>
        /// 过期时间，单位小时，-1表示不判断过期
        /// </summary>
        public int expire_hours { get; set; }
    }
}
