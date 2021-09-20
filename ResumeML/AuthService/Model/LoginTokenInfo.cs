using System;

namespace AuthService
{
    public class LoginTokenInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 令牌签发时间
        /// </summary>
        public long TokenStart { get; set; }
        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public long TokenExpire { get; set; }
    }
}
