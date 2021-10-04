using MyAccess.Json.Attributes;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class Data_Permission
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string code { get; set; }
        [JsonIgnore]
        public int sort { get; set; }
        /// <summary>
        /// 子权限
        /// </summary>
        [JsonIgnore(val: null)]
        public List<Data_Permission> children { get; set; }
    }
}
