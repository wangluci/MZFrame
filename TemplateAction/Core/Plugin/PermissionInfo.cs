using System;


namespace TemplateAction.Core
{
    /// <summary>
    /// 注解的权限信息
    /// </summary>
    public class PermissionInfo
    {
        public PermissionInfo()
        {
            Name = string.Empty;
            Code = string.Empty;
            ParentCode = string.Empty;
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
    }
}
