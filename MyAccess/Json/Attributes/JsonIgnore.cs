
using System;
namespace MyAccess.Json.Attributes
{
    public class JsonIgnore : JsonAttr
    {
        /// <summary>
        /// 返回true不显示
        /// </summary>
        /// <param name="input">原值</param>
        /// <returns></returns>
        public delegate bool HideFun(object input);
        public HideFlag Flag { get; set; }
        private HideFun _fun;
        public HideFun Fun { get { return _fun; } }

        /// <summary>
        /// 不显示
        /// </summary>
        public JsonIgnore()
        {
            Flag = HideFlag.Hide;
        }
        /// <summary>
        /// 指定条件不显示
        /// </summary>
        /// <param name="fun"></param>
        public JsonIgnore(HideFun fun)
        {
            _fun = fun;
            Flag = HideFlag.HideCondition;
        }
        /// <summary>
        /// 指定值不显示
        /// </summary>
        /// <param name="val"></param>
        public JsonIgnore(object val)
        {
            _fun = (input) =>
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
            Flag = HideFlag.HideCondition;
        }
    }
}
