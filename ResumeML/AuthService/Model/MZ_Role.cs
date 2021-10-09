using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;

namespace AuthService
{
    public class MZ_Role
    {
        [JsonPropertyName("key")]
        [ID(true)]
        public virtual long RoleID { get; set; }
        [JsonPropertyName("name")]
        public virtual string RoleName { get; set; }
        [JsonPropertyName("description")]
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
