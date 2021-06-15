
using System;
namespace MyAccess.Json.Attributes
{
    public class JsonIgnore : JsonAttr
    {
        public HideFlag Flag { get; set; }
        public JsonIgnore(HideFlag flag = 0)
        {
            Flag = flag;
        }
    }
}
