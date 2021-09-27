using System;

namespace AuthService
{
    public class UserPermission
    {
        public UserPermission()
        {
            UserId = 0;
            RightCode = "";
            RightType = 0;
        }
        public long UserId { get; set; }
        public string RightCode { get; set; }
        /// <summary>
        /// 0为允许，1为禁止
        /// </summary>
        public int RightType { get; set; }
    }
}
