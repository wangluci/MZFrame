using MyAccess.DB.Attr;
using MyAccess.Json.Attributes;
using System;

namespace AuthService
{
    public class AdminInfo
    {
        [ID(true)]
        public virtual long Id { get; set; }
        /// <summary>
        /// 后台用户名
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public virtual string RealName { get; set; }
        /// <summary>
        /// 后台密码
        /// </summary>
        [JsonIgnore()]
        public virtual string Password { get; set; }
        public virtual string Avatar { get; set; }
        public virtual string Introduction { get; set; }
        /// <summary>
        /// 绑定对象
        /// </summary>
        public virtual string BindId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }
    }
}
