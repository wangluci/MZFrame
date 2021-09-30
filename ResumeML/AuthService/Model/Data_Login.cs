using System;

namespace AuthService
{
    public class Data_Login
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long expire { get; set; }
    }
}
