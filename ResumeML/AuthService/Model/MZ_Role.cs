using MyAccess.DB.Attr;
using MyAccess.Json.Attributes;
using System;

namespace AuthService
{
    public class MZ_Role
    {
        [JsonName("key")]
        [ID(true)]
        public virtual long RoleID { get; set; }
        [JsonName("name")]
        public virtual string RoleName { get; set; }
        [JsonName("description")]
        public virtual string RoleDesc { get; set; }
        [JsonIgnore]
        public virtual int RoleType { get; set; }
        [JsonIgnore]
        public virtual long CreateUserId { get; set; }
        [JsonIgnore]
        public virtual DateTime CreateDate { get; set; }
        [DataIgnore]
        public string[] permissions { get; set; }
    }
}
