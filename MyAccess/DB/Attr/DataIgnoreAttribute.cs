using System;
namespace MyAccess.DB.Attr
{
    /// <summary>
    /// 表示此字段不是数据库字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataIgnoreAttribute : Attribute
    {
    }
}
