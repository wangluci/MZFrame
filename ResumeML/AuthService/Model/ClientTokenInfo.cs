using System;

namespace AuthService
{
    public class ClientTokenInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public long Expire { get; set; }
    }
}
