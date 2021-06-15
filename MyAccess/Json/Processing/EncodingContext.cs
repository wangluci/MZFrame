using System;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// 
    /// </summary>
    internal class EncodingContext : Context
    {
        /// <summary>
        /// Value to encode.
        /// </summary>
        internal object Value { get; set; }

        /// <summary>
        /// Available strong type, otherwise null.
        /// </summary>
        internal Type KnownType { get; private set; }

        /// <summary>
        /// Json token sequence for writing output to.
        /// </summary>
        internal JsonTokenSequence Output { get; set; }
        /// <summary>
        /// 是否可以为NULL
        /// </summary>
        internal bool UseNullable { get; set; }
        /// <summary>
        /// 是否使用unicode编码汉字
        /// </summary>
        internal bool UseUnicode { get; set; }
        /// <summary>
        /// 日期格式化
        /// </summary>
        internal IDateTimeFormat DateTimeFormat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="value"></param>
        /// <param name="knownType"></param>
        internal EncodingContext(Process process, object value, Type knownType) : base(process)
        {
            this.Value = value;
            this.KnownType = knownType;

            this.Output = new JsonTokenSequence();
        }

    }
}
