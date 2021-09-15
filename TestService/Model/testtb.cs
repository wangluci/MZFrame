using MyAccess.DB.Attr;
using MyAccess.Json.Attributes;
using System;

namespace TestService.Model
{
    public class testtb
    {
        [ID(true)]
        public virtual int testid { get; set; }
        [JsonName("td")]
        public virtual string testdes { get; set; }
    }
}
