
namespace MyAccess.Json.Attributes
{
    public class JsonDefault : JsonAttr
    {
        private object _srcVal;
        private object _defaultVal;
        public object Default
        {
            get { return _defaultVal; }
        }
        public object Src
        {
            get { return _srcVal; }
        }
        /// <summary>
        /// 当值等于src时，赋值val
        /// </summary>
        /// <param name="src"></param>
        /// <param name="val"></param>
        public JsonDefault(object src,object val)
        {
            _srcVal = src;
            _defaultVal = val;
        }
        public JsonDefault(object val)
        {
            _srcVal = null;
            _defaultVal = val;
        }
    }
}
