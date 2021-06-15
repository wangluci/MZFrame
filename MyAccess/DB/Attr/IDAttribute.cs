using System;

namespace MyAccess.DB.Attr
{
    /// <summary>
    /// 标识ID作用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IDAttribute : Attribute
    {
        /// <summary>
        /// 判断是否为自增ID
        /// </summary>
        private bool _auto;
        public bool IsAuto { get { return _auto; } }
        /// <summary>
        /// 标记是否为主键，默认为非自增
        /// </summary>
        public IDAttribute()
        {
            _auto = false;
        }
        /// <summary>
        /// 可设置是否为自增
        /// </summary>
        /// <param name="auto"></param>
        public IDAttribute(bool auto)
        {
            _auto = auto;
        }
    }
}
