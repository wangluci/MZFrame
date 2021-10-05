using MyAccess.DB.Attr;
using MyAccess.Json.Attributes;
using System;

namespace AuthService
{
    public class MZ_Role
    {
        [JsonName("key")]
        [ID(true)]
        public long RoleID { get; set; }
        [JsonName("name")]
        public string RoleName { get; set; }
        [JsonName("description")]
        public string RoleDesc { get; set; }
        [JsonIgnore]
        public int RoleType { get; set; }
        [JsonIgnore]
        public long CreateUserId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }
}
