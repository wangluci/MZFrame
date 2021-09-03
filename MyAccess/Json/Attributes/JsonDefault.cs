
namespace MyAccess.Json.Attributes
{
    public class JsonDefault : JsonAttr
    {
        /// <summary>
        /// 返回要替换的值
        /// </summary>
        /// <param name="input">原值</param>
        /// <returns></returns>
        public delegate object ReplaceFun(object input);
        private ReplaceFun _fun;
        internal ReplaceFun Fun
        {
            get { return _fun; }
        }

        /// <summary>
        /// 替换指定值
        /// </summary>
        /// <param name="fun"></param>
        public JsonDefault(ReplaceFun fun)
        {
            _fun = fun;
        }
        /// <summary>
        /// 当值等于null时，赋值val
        /// </summary>
        /// <param name="val"></param>
        public JsonDefault(object val)
        {
            _fun = (input) =>
            {
                if (input == null)
                {
                    return val;
                }
                else
                {
                    return input;
                }
            };
        }
    }
}
