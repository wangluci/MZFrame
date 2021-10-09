using System;
namespace MyAccess.Json.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public abstract class JsonAttr : Attribute
    {
        public abstract bool DecodeBind(ref object key, ref object val);
    }
}
