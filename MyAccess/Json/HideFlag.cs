using System;
namespace MyAccess.Json
{
    [Flags]
    public enum HideFlag
    {
        /// <summary>
        /// 不显示
        /// </summary>
        Hide = 0,
        /// <summary>
        /// 指定值不显示
        /// </summary>
        ValueHide = 1
    }
}
