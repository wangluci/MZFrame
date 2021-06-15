using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MyAccess.Filter
{
    public class HtmlFilter
    {
        private static readonly RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        // 依次填入上文中三个正则表达式
        private static readonly Regex TAG_REGEX = new Regex(@"<\/?.+?>", REGEX_OPTIONS);
        private static readonly Regex VALID_TAG_REGEX = new Regex(@"^(?<begin></?)(?<tag>[a-zA-z]+)\s*(?<attr>[^>]*?)(?<end>/?>)$", REGEX_OPTIONS);
        private static readonly Regex ATTRIBUTE_REGEX = new Regex(@"(?<name>[a-zA-Z\-]+)\s*=\s*""(?<value>[^""]*)""", REGEX_OPTIONS);

        public HtmlFilter() : this(null) { }

        public HtmlFilter(FilterConfig config)
        {
            this.Config = config ?? new FilterConfig();
        }

        public FilterConfig Config { get; private set; }
        /// <summary>
        /// 清除html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ClearHtml(string html, bool htmldecode)
        {
            if (htmldecode)
            {
                return System.Net.WebUtility.HtmlDecode(Regex.Replace(html, @"<\/?.+?>", ""));
            }
            else
            {
                return Regex.Replace(html, @"<\/?.+?>", "");
            }
        }
        public static string ClearHtml(string html)
        {
            return ClearHtml(html, false);
        }
        public static string ClearScript(string html)
        {
            return Regex.Replace(html, "(<script)[\\s\\S]*?(</script>)|(<style)[\\s\\S]*?(</style>)", "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 清除html和表情
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ClearHtmlWithEmoji(string html, bool htmldecode)
        {
            html = Regex.Replace(html, @"\[em_([0-9]*)\]", "");//清除表情
            return ClearHtml(html, htmldecode);
        }
        public static string ClearHtmlWithEmoji(string html)
        {
            return ClearHtmlWithEmoji(html, false);
        }
        public static string OnlyText(string input)
        {
            input = ClearHtmlWithEmoji(input, true);
            return Regex.Replace(input, @"[^\u4e00-\u9fa5a-zA-z0-9\s]*", "");
        }

        private static string EscapeToUtf32(string str)
        {
            List<string> escaped = new List<string>();
            List<int> unicodeCodes = StringToUnicodeCodePoints(str);
            string hex;

            for (int i = 0; i < unicodeCodes.Count; i++)
            {
                hex = unicodeCodes[i].ToString("X");
                if (hex.Length > 4)
                {
                    escaped.Add(hex);
                }
                else
                {
                    escaped.Add("0000".Substring(hex.Length) + hex);
                }
            }
            return string.Join("-", escaped);
        }
        private static List<int> StringToUnicodeCodePoints(string str)
        {
            ushort surrogate1st = 0;
            List<int> unicodeCodes = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                ushort utf16Code = str[i];
                if (surrogate1st != 0)
                {
                    if (utf16Code >= 0xDC00 && utf16Code <= 0xDFFF)
                    {
                        int unicodeCode = (surrogate1st - 0xD800) * (1 << 10) + (1 << 16) + (utf16Code - 0xDC00);
                        unicodeCodes.Add(unicodeCode);
                    }
                    surrogate1st = 0;
                }
                else if (utf16Code >= 0xD800 && utf16Code <= 0xDBFF)
                {
                    surrogate1st = utf16Code;
                }
                else
                {
                    unicodeCodes.Add(utf16Code);
                }
            }
            return unicodeCodes;
        }

        public static string EmojiEncode(string str)
        {
            MatchCollection mc = Regex.Matches(str, "(\uD83C[\uDDE8-\uDDFF]\uD83C[\uDDE7-\uDDFF])|[\uD800-\uDBFF][\uDC00-\uDFFF]|[\u2600-\u27ff][\uFE0F]|[\u2600-\u27ff]");

            StringBuilder sb = new StringBuilder();
            int apindex = 0;
            foreach (Match m in mc)
            {
                sb.Append(str.Substring(apindex, m.Index - apindex));
                apindex = m.Index + m.Length;
                sb.Append("&#x" + EscapeToUtf32(m.Value) + ";");
            }
            sb.Append(str.Substring(apindex, str.Length - apindex));
            return sb.ToString();
        }
        /// <summary>
        /// SQL关键词过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlLikeFilter(string str)
        {
            StringBuilder strbuild = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '\"' || chr == '\'' || chr == '%' || chr == '[' || chr == ']' || chr == '_')
                {
                    continue;
                }
                strbuild.Append(chr);
            }

            return strbuild.ToString();
        }
        public static string JsonTxtFilter(string str)
        {
            StringBuilder strbuild = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '\'' || chr == '\"' || chr == '<' || chr == '>')
                {
                    continue;
                }
                else if (chr == '\r')
                {
                    strbuild.Append("\\r");
                }
                else if (chr == '\n')
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
        /// 获取标签个数
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int GetTagCount(string tag)
        {
            TagConfig outConfig = null;
            if (Config.TryGetValue(tag, out outConfig))
            {
                return outConfig.TagCount;
            }
            return 0;
        }

        /// <summary>
        /// 获取指定标签的指定属性数
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public int GetTagAttrCount(string tag, string attr)
        {
            TagConfig outConfig = null;
            if (Config.TryGetValue(tag, out outConfig))
            {
                return outConfig.GetAttrCount(attr);
            }
            return 0;
        }
        /// <summary>
        /// 标签个数超出将抛出异常
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string FilterHtml(string html)
        {
            foreach (TagConfig tc in this.Config.Values)
            {
                tc.ResetCount();
            }
            // 对每个HTML标记进行替换?
            string rt = TAG_REGEX.Replace(html, GetTag);
            foreach (KeyValuePair<string, TagConfig> kvp in this.Config)
            {
                if (kvp.Value.NeedCloseNum > 0)
                {
                    throw new Exception(string.Format("{0}标签无结束标记", kvp.Key));
                }
            }
            return rt;
        }

        private string GetTag(Match match)
        {
            // 如果不是合法的HTML标记形式，则替换为空字符串
            Match validTagMatch = VALID_TAG_REGEX.Match(match.Value);
            if (!validTagMatch.Success) return "";

            string tag = validTagMatch.Groups["tag"].Value;

            // 如果这个标记不在白名单中，则替换为空字符串
            TagConfig tagConfig;
            if (!this.Config.TryGetValue(tag, out tagConfig)) return "";

            string begin = validTagMatch.Groups["begin"].Value;
            // 如果是闭合标记，则直接构造并返回
            if (begin == "</")
            {
                tagConfig.NeedCloseNum--;
                return String.Format("</{0}>", tag.ToLower());
            }

            tagConfig.Increase();
            if (tagConfig.TagCount > tagConfig.MaxCount && tagConfig.MaxCount > 0)
            {
                throw new Exception(string.Format("标签{0}个数不能大于{1}", tag, tagConfig.MaxCount));
            }

            // 过滤出合法的属性键值对
            string attrText = validTagMatch.Groups["attr"].Value;
            MatchCollection attrMatches = ATTRIBUTE_REGEX.Matches(attrText);
            List<string> arrlist = new List<string>();
            for (int i = 0; i < attrMatches.Count; i++)
            {
                string attrstr = GetAttribute(attrMatches[i], tagConfig);
                if (!string.IsNullOrEmpty(attrstr))
                {
                    arrlist.Add(attrstr);
                }
            }

            string end = validTagMatch.Groups["end"].Value;
            if (!end.StartsWith("/"))
            {
                tagConfig.NeedCloseNum++;
            }
            // 如果没有合法的属性，则直接构造返回
            if (arrlist.Count == 0)
            {
                return begin + tag + end;
            }
            else // 否则返回带属性的HTML标记
            {
                return String.Format(
                    "{0}{1} {2}{3}",
                    begin,
                    tag,
                    String.Join(" ", arrlist.ToArray()),
                    end);
            }
        }

        private static string GetAttribute(Match attrMatch, TagConfig tagConfig)
        {
            string name = attrMatch.Groups["name"].Value;

            Regex regex;
            if (!tagConfig.TryGetValue(name, out regex)) return "";
            //增加属性数
            tagConfig.IncreaseAttrCount(name);

            string value = attrMatch.Groups["value"].Value;
            if (regex.IsMatch(value))
            {
                return String.Format("{0}=\"{1}\"", name, value);
            }
            else
            {
                return "";
            }
        }
    }
}
