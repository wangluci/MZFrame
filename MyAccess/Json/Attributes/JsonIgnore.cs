﻿
using System;
namespace MyAccess.Json.Attributes
{
    /// <summary>
    /// 指定字段不编码并和不解码
    /// </summary>
    public class JsonIgnore : JsonAttr
    {
        public override bool DecodeBind(ref object key, ref object val)
        {
            return true;
        }
    }
}
