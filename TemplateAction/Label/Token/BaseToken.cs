﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成
//     如果重新生成代码，将丢失对此文件所做的更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
namespace TemplateAction.Label.Token
{
    public abstract class BaseToken
    {
        protected string mValue;
        protected string mToken;
        /// <summary>
        /// 标记种类，一般是首字符，结构体则为value值
        /// </summary>
        public string Token
        {
            get { return mToken; }
        }
        public int Length
        {
            get
            {
                return mValue.Length;
            }
        }
        public override string ToString()
        {
            if (mValue != null)
            {
                return mValue;
            }
            else
            {
                return string.Empty;
            }
        }
        public BaseToken(string token)
        {
            mValue = token;
            mToken = token;
        }
        public T Value<T>()
        {
            object val = GetValue();
            Type type = val.GetType();
            Type expectedType = typeof(T);
            if (!Common.TAUtility.IsAs(type, expectedType))
            {
                if (Common.TAUtility.IsNumerical(expectedType) && val is double)
                {
                    if (expectedType == typeof(byte)) val = Convert.ToByte(val);
                    else if (expectedType == typeof(sbyte)) val = Convert.ToSByte(val);
                    else if (expectedType == typeof(short)) val = Convert.ToInt16(val);
                    else if (expectedType == typeof(ushort)) val = Convert.ToUInt16(val);
                    else if (expectedType == typeof(int)) val = Convert.ToInt32(val);
                    else if (expectedType == typeof(uint)) val = Convert.ToUInt32(val);
                    else if (expectedType == typeof(long)) val = Convert.ToInt64(val);
                    else if (expectedType == typeof(ulong)) val = Convert.ToUInt64(val);
                    else if (expectedType == typeof(float)) val = Convert.ToSingle(val);
                }
            }
            return (T)val;
        }
        public abstract object GetValue();
        public abstract bool Append(char value);
        public abstract string Type { get; }
    }
}

