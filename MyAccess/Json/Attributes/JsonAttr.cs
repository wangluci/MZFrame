using System;
namespace MyAccess.Json.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class JsonAttr : Attribute
    {
    }
}
