using System;

namespace AuthService
{
    public class LoginToken
    {
        /// <summary>
        /// 令牌信息
        /// </summary>
        public LoginTokenInfo Info { get; set; }
        /// <summary>
        /// 签名数据
        /// </summary>
        public string Sign { get; set; }
    }
}
