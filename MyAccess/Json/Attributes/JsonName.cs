using System;

namespace MyAccess.Json.Attributes
{
    /// <summary>
    /// 定义json解释与编码的名称
    /// </summary>
    public class JsonName : JsonAttr
    {
        private string _name;
        public string Name { get { return _name; } }
        public JsonName(string name)
        {
            _name = name;
        }
        public override bool DecodeBind(ref object key, ref object val)
        {
            key = _name;
            return false;
        }
    }
}
