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
        public int RightType { get; set; }
    }
}
