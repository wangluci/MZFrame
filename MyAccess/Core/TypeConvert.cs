using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyAccess.Core
{
    public class TypeConvert
    {
        public static string DictToUrl(Dictionary<string, object> dict)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in dict)
            {
                if (pair.Value != null)
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        public static Dictionary<string, object> UrlToDict(string url)
        {
            string[] xdicts = url.Split('&');
            Dictionary<string, object> dics = new Dictionary<string, object>();
            foreach (string xs in xdicts)
            {
                int kvidx = xs.IndexOf("=");
                if (kvidx > 0)
                {
                    string tkey = xs.Substring(0, kvidx);
                    string tval = xs.Substring(kvidx + 1);
                    dics.Add(tkey, tval);
                }
            }
            return dics;
        }

        /// <summary>
        /// java时间转C#
        /// </summary>
        /// <param name="time_JAVA_Long"></param>
        /// <returns></returns>
        public static DateTime JavaLongTime2CSharp(long time_JAVA_Long)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(time_JAVA_Long * 10000);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        /// <summary>
        /// C#时间转java
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long Time2JavaLong(DateTime time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = time - dtStart;
            return toNow.Ticks / 10000;
        }
        /// <summary>
        /// 长整型转36进制激活码
        /// </summary>
        /// <returns></returns>
        public static string ToCode(long input)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string a = "";
            while (input >= 1)
            {
                int index = Convert.ToInt16(input - (input / 36) * 36);
                a = chars[index] + a;
                input = input / 36;
            }
            return a;
        }
        public static bool isNull(object value)
        {
            return (value == null || Convert.IsDBNull(value)) ? true : false;
        }


        /// <summary>
        /// 数据转换类
        /// </summary>
        #region 转换对像为布尔值
        /// <summary>
        /// 转换对象为布尔值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的布尔值</returns>
        public static bool StrToBool(object Value)
        {
            if (!isNull(Value))
            {
                string[] array = new string[] { "true", "yes", "1" };
                return (Array.IndexOf<string>(array, Value.ToString().ToLower()) >= 0);
            }
            return false;
        }

        /// <summary>
        /// 转换对象为布尔值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>转换后的布尔值</returns>
        public static bool StrToBool(object Value, bool DefaultValue)
        {
            if (!isNull(Value))
            {
                string[] array = new string[] { "true", "yes", "1" };
                return ((Array.IndexOf<string>(array, Value.ToString().ToLower()) >= 0));
            }
            return DefaultValue;
        }

        #endregion

        #region 转换对象为字符串
        /// <summary>
        /// 转换对象为字符串
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的字符串</returns>
        public static string ToString(object Value)
        {
            if (isNull(Value))
            {
                return string.Empty;
            }
            return Value.ToString();
        }

        /// <summary>
        /// 转换对象为字符串
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <param name="Trim">是否去除空格</param>
        /// <returns>转换后的字符串</returns>
        public static string ToString(object Value, string DefaultValue, bool Trim)
        {
            if (isNull(Value))
            {
                return ToString(DefaultValue);
            }
            string tRtStr = Value.ToString();
            if (tRtStr == string.Empty)
            {
                return ToString(DefaultValue);
            }
            if (Trim)
            {
                return tRtStr.Trim();
            }
            return tRtStr;
        }
        public static string ToString(object Value, string DefaultValue)
        {
            return ToString(Value, DefaultValue, true);
        }

        #endregion

        #region 将对象转换为Int32类型


        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ToInt(string str)
        {
            return ToInt(str, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ToInt(string str, int defValue)
        {
            int rt = defValue;
            if (str != null)
            {
                if (!int.TryParse(str, out rt))
                {
                    rt = defValue;
                }
            }
            return rt;
        }
        /// <summary>
        /// 转换对象为整形数值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的整形数值</returns>
        public static int ToInt(object Value)
        {
            return ToInt(Value, 0);
        }

        /// <summary>
        /// 转换对象为整形数值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>转换后的整形数值</returns>
        public static int ToInt(object Value, int DefaultValue)
        {
            int rt = DefaultValue;
            if (Value != null)
            {
                if (!int.TryParse(Value.ToString(), out rt))
                {
                    rt = DefaultValue;
                }
            }
            return rt;
        }
        #endregion

        #region 转换为float型
        /// <summary>
        /// object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue)
        {
            return ObjectToFloat(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue)
        {
            return StrToFloat(strValue, 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {

            float rt = defValue;
            if (strValue != null)
            {
                if (!float.TryParse(strValue, out rt))
                {
                    rt = defValue;
                }
            }
            return rt;
        }
        #endregion

        #region 转换为Decimal

        /// <summary>
        /// object转换为Decimal
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <returns>返回Decimal</returns>
        public static decimal CDecimal(object input)
        {
            return CDecimal(input, 0M);
        }

        /// <summary>
        /// object转换为Decimal
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Decimal</returns>
        public static decimal CDecimal(object input, decimal defaultValue)
        {
            if (!isNull(input))
            {
                return CDecimal(input.ToString(), 0M);
            }
            return 0M;
        }

        /// <summary>
        /// string转换为Decimal
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <returns>返回Decimal</returns>
        public static decimal CDecimal(string input)
        {
            return CDecimal(input, 0M);
        }

        /// <summary>
        /// string转换为Decimal
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Decimal</returns>
        public static decimal CDecimal(string input, decimal defaultValue)
        {
            decimal num;
            if (!decimal.TryParse(input, out num))
            {
                num = defaultValue;
            }
            return num;
        }

        #endregion

        #region 转换为Double
        /// <summary>
        /// object转换为Double
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <returns>返回Double</returns>
        public static double CDouble(object input)
        {
            return CDouble(input, 0.0);
        }

        /// <summary>
        /// object转换为Double
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Double</returns>
        public static double CDouble(object input, double defaultValue)
        {
            if (!isNull(input))
            {
                return CDouble(input.ToString(), defaultValue);
            }
            return 0.0;
        }

        /// <summary>
        /// string转换为Double
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <returns>返回Double</returns>
        public static double CDouble(string input)
        {
            return CDouble(input, 0.0);
        }

        /// <summary>
        /// string转换为Double
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Double</returns>
        public static double CDouble(string input, double defaultValue)
        {
            double num;
            if (!double.TryParse(input, out num))
            {
                return defaultValue;
            }
            return num;
        }

        #endregion

        #region 转换为Long类型
        /// <summary>
        /// object转换为Long类型
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <returns>返回Long类型</returns>
        public static int CLng(object input)
        {
            return CLng(input, 0);
        }

        /// <summary>
        /// object转换为Long类型
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Long类型</returns>
        public static int CLng(object input, int defaultValue)
        {
            if (!isNull(input))
            {
                return CLng(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// string转换为Long类型
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <returns>返回Long类型</returns>
        public static int CLng(string input)
        {
            return CLng(input, 0);
        }

        /// <summary>
        /// string转换为Long类型
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Long类型</returns>
        public static int CLng(string input, int defaultValue)
        {
            int num;
            if (!int.TryParse(input, out num))
            {
                num = defaultValue;
            }
            return num;
        }

        #endregion

        #region 转换为Single
        /// <summary>
        /// object转换为Single
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <returns>返回Single</returns>
        public static float CSingle(object input)
        {
            return CSingle(input, 0f);
        }

        /// <summary>
        /// object转换为Single
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Single</returns>
        public static float CSingle(object input, float defaultValue)
        {
            if (!isNull(input))
            {
                return CSingle(input.ToString(), defaultValue);
            }
            return 0f;
        }

        /// <summary>
        /// string转换为Single
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <returns>返回Single</returns>
        public static float CSingle(string input)
        {
            return CSingle(input, 0f);
        }

        /// <summary>
        /// string转换为Single
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回Single</returns>
        public static float CSingle(string input, float defaultValue)
        {
            float num;
            if (!float.TryParse(input, out num))
            {
                num = defaultValue;
            }
            return num;
        }

        #endregion






        #region 转换对象为长整形数值
        /// <summary>
        /// 转换对象为长整形数值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的长整形数值</returns>
        public static long ToLong(object Value)
        {
            return ToLong(Value, 0L);
        }


        /// <summary>
        /// 转换对象为长整形数值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>转换后的长整形数值</returns>
        public static long ToLong(object Value, long DefaultValue)
        {
            if (!isNull(Value))
            {
                long rt;
                if (long.TryParse(Value.ToString().Trim(), out rt))
                {
                    return rt;
                }
            }
            return DefaultValue;
        }
        #endregion

        #region 转换对象为日期值
        public static DateTime CDate(object input, DateTime def)
        {
            if (!isNull(input))
            {
                return CDate(input.ToString(), def);
            }
            return def;
        }
        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="input">对象</param>
        /// <returns>转换后的日期值</returns>
        public static DateTime CDate(object input)
        {
            return CDate(input, DateTime.Now);
        }
        public static DateTime CDate(string input)
        {
            return CDate(input, DateTime.Now);
        }
        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="input">日期型字符串</param>
        /// <returns>转换后的日期值</returns>
        public static DateTime CDate(string input, DateTime def)
        {
            DateTime now = def;
            if (!DateTime.TryParse(input, out now))
            {
                now = def;
            }
            return now;
        }

        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="input">日期型字符串</param>
        /// <param name="outTime">默认值</param>
        /// <returns>转换后的日期值</returns>
        public static DateTime? CDate(string input, DateTime? outTime)
        {
            DateTime time;
            if (!DateTime.TryParse(input, out time))
            {
                return outTime;
            }
            return new DateTime?(time);
        }

        #endregion

        #region int型转换为string型
        /// <summary>
        /// int型转换为string型
        /// </summary>
        /// <returns>转换后的string类型结果</returns>
        public static string IntToStr(int intValue)
        {
            return Convert.ToString(intValue);
        }
        #endregion


        public static int[] StrToIntArr(string input)
        {
            string[] strArr = input.Split(new char[] { ',' });
            int[] intArr = new int[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                string str = strArr[i];
                intArr[i] = ToInt(str, 0);
            }
            return intArr;
        }
        public static List<string> StrToList(string input)
        {
            string[] strArr = input.Split(new char[] { ',' });
            List<string> _list = new List<string>();
            foreach (string str in strArr)
            {
                _list.Add(str);
            }
            return _list;
        }
        public static string ListToStr(List<string> arr)
        {
            string _output = "";
            for (int i = 0; i < arr.Count; i++)
            {
                if (i == 0)
                {
                    _output += arr[i];
                }
                else
                {
                    _output += "," + arr[i];
                }
            }
            return _output;
        }
        public static int Round(double input)
        {
            return (int)ToRound(input, 0);
        }
        /// <summary>
        /// 真正四舍五入
        /// </summary>
        /// <param name="input"></param>
        /// <param name="accuracy">要进的位</param>
        /// <returns></returns>
        public static float ToRound(double input, int accuracy)
        {
            float k = 1;
            for (int i = 0; i < accuracy; i++)
                k *= 10;
            int a = (int)(input * k + 0.5);
            return a / k;
        }
    }
}
