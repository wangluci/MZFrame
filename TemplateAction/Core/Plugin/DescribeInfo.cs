using System;


namespace TemplateAction.Core
{
    /// <summary>
    /// 描述信息
    /// </summary>
    public class DescribeInfo
    {
        public DescribeInfo()
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
