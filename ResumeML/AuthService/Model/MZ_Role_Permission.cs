using MyAccess.DB.Attr;
using System;

namespace AuthService
{
    public class MZ_Role_Permission
    {
        [ID(true)]
        public long RoleRightID { get; set; }
        public long RoleID { get; set; }
        public string RightCode { get; set; }
    }
}
