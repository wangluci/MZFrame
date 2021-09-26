using System;

namespace AuthService
{
    /// <summary>
    /// 用户返回信息
    /// </summary>
    public class Data_UserInfo
    {
        public string name { get; set; }
        public string avatar { get; set; }
        public string introduction { get; set; }
        public string[] roles { get; set; }
    }
}
