
namespace MyAccess.Json.Attributes
{
    public class JsonDefault: JsonAttr
    {
        private object _defaultVal;
        public object Default
        {
            get { return _defaultVal; }
        }
        public JsonDefault(object val)
        {
            _defaultVal = val;
        }
    }
}
