using System;

namespace MyAccess.Json.Attributes
{
    public class JsonHide : JsonAttr
    {
        /// <summary>
        /// 返回true不显示
        /// </summary>
        /// <param name="input">原值</param>
        /// <returns></returns>
        public delegate bool HideFun(object input);
        public HideFun Fun { get; set; }

        /// <summary>
        /// 指定值不编码
        /// </summary>
        /// <param name="val"></param>
        public JsonHide(object val = null)
        {
            Fun = (input) =>
            {
                if (input == val)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }

        public override bool DecodeBind(ref object key, ref object val)
        {
            if (Fun(val))
            {
                return true;
            }
            return false;
        }
    }
}
