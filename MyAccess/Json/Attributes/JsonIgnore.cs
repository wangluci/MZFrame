
using System;
namespace MyAccess.Json.Attributes
{
    public class JsonIgnore : JsonAttr
    {
        public object Val { get; }
        public HideFlag Flag { get; set; }
        public JsonIgnore(HideFlag flag = HideFlag.Hide)
        {
            Flag = flag;
        }
        public JsonIgnore(object val)
        {
            Val = val;
            Flag = HideFlag.ValueHide;
        }
    }
}
