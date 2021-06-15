using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace TemplateAction.Common
{
    public class TAUtility
    {
        /// <summary>
        /// 全局定义
        /// </summary>
        public const string FILE_EXT = ".my.html";
        public const string FUN_VAR = "var";
        public const string ASSIGN_SRC = "src";
        public const string CONDITION_EX = "ex";
        public const string FOR_FROM = "from";
        public const string FOR_NAME = "name";
        public const string FOR_INDEX = "index";
        public const string NS_KEY = "namespace";
        public const string CONTROLLER_KEY = "controller";
        public const string ACTION_KEY = "action";
        public const string HTML_ENCODE = "html";
        /// <summary>
        /// 判断是否为静态文件url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsStaticFile(string url)
        {
            if (url.Length <= 0) { return false; }
            if (url[url.Length - 1] == '.')
            {
                return false;
            }
            for (int i = url.Length - 1; i >= 0; i--)
            {
                char c = url[i];
                if (!char.IsLetter(c))
                {
                    if (c == '.')
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 判断是否为变量名
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool NoVarChar(char c)
        {
            return (c < 65 || c > 122 || (c > 90 && c < 97)) && c != 46 && c != 95;
        }

        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2) r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            return r;
        }
        public static string ToStr(object value, string defaultValue)
        {
            string rt = defaultValue;
            if (value != null)
            {
                return value.ToString();
            }
            return rt;
        }

        public static int ReadFile(out string cont, string path)
        {
            return ReadFile(out cont, path, Encoding.UTF8);
        }
        public static int ReadFile(out string cont, string path, Encoding def)
        {
            try
            {
                string str = string.Empty;
                if (File.Exists(path))
                {

                    StreamReader reader = new StreamReader(path, def);
                    str = reader.ReadToEnd();
                    reader.Close();
                    cont = str;
                    return 0;
                }
                cont = "文件不存在";
                return -1;
            }
            catch
            {
                cont = "读取文件出错";
                return -2;
            }
        }
        public static byte[] ReadBinFile(string path)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                return buffur;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    //关闭资源
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 判断是否为数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumerical(Type type)
        {
            return (type.IsPrimitive) && type != typeof(bool) && type != typeof(char);
        }
        /// <summary>
        /// 判断type是否可转换成type,支持泛型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asType"></param>
        /// <returns></returns>
        public static bool IsAs(Type type, Type asType)
        {
            if (asType.IsAssignableFrom(type)) return true;
            if (type.IsGenericType)
            {
                Type typeReduced = type.GetGenericTypeDefinition();
                Type asTypeReduced = asType;

                if (asType.IsGenericType)
                {
                    asTypeReduced = asType.GetGenericTypeDefinition();
                }
                if (asTypeReduced.IsAssignableFrom(typeReduced))
                {
                    Type[] typeArguments = type.GetGenericArguments();
                    Type[] asTypeArguments = asType.GetGenericArguments();
                    if (typeArguments.Length != asTypeArguments.Length) return false;

                    bool isSuccess = true;
                    for (int i = 0; i < typeArguments.Length; i++)
                    {
                        if (typeArguments[0].IsGenericParameter)
                        {
                            isSuccess = false;
                            break;
                        }
                        if (!asTypeArguments[0].IsGenericParameter)
                        {
                            isSuccess = false;
                            break;
                        }
                    }

                    if (isSuccess)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string BSubStr(string s, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            int n = 0;
            int i = 0;
            for (; i < bytes.GetLength(0) && n < length; i++)
            {
                if (i % 2 == 0)
                {
                    n++;
                }
                else
                {
                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }

            }

            if (i % 2 == 1)
            {
                if (bytes[i] > 0)
                    i = i - 1;
                else
                    i = i + 1;
            }
            return System.Text.Encoding.Unicode.GetString(bytes, 0, i);
        }


        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AllFilter(string str)
        {
            StringBuilder strbuild = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '\'' || chr == '\"' || chr == '<' || chr == '>')
                {
                    continue;
                }
                else if(chr == '\r')
                {
                    strbuild.Append("\\r");
                }
                else if(chr == '\n')
                {
                    strbuild.Append("\\n");
                }
                else
                {
                    strbuild.Append(chr);
                }
           
            }
            return strbuild.ToString();
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


        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// 对称解密
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string encrypted, string key)
        {
            byte[] buffer = Convert.FromBase64String(encrypted);
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            return Encoding.UTF8.GetString(Decrypt(buffer, bytes));
        }

        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }
        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string original, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] buffer2 = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(Encrypt(bytes, buffer2));
        }
        public static string Unicode(string input)
        {
            StringBuilder parsed = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '"':
                        parsed.Append("\\\"");
                        break;
                    case '\\':
                        parsed.Append("\\\\");
                        break;
                    case '/':
                        parsed.Append("\\/");
                        break;
                    case '\b':
                        parsed.Append("\\b");
                        break;
                    case '\f':
                        parsed.Append("\\f");
                        break;
                    case '\n':
                        parsed.Append("\\n");
                        break;
                    case '\r':
                        parsed.Append("\\r");
                        break;
                    case '\t':
                        parsed.Append("\\t");
                        break;
                    default:
                        if (c < ' ' || c > 127)
                        {
                            parsed.Append("\\u" + ((uint)c).ToString("X4"));
                        }
                        else
                        {
                            parsed.Append(c);
                        }
                        break;
                }
            }
            return parsed.ToString();
        }


    }
}
