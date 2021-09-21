using System;

namespace AuthService
{
    public class ClientToken
    {
        /// <summary>
        /// 令牌信息
        /// </summary>
        public ClientTokenInfo Info { get; set; }
        /// <summary>
        /// 签名数据
        /// </summary>
        public string Sign { get; set; }
    }
}
