using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace MyAccess.Filter
{
    public class FilterConfig : Dictionary<string, TagConfig>
    {

        public FilterConfig()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            //匹配标签的style,class
            TagConfig allStar = CreateAllStyle();

            TagConfig divTag = new TagConfig(150);
            divTag.CopyFrom(allStar);
            this.Add("div", divTag);


            TagConfig pTag = new TagConfig(100);
            pTag.CopyFrom(allStar);
            this.Add("p", pTag);

            TagConfig strongTag = new TagConfig(100);
            strongTag.CopyFrom(allStar);
            this.Add("strong", strongTag);

            TagConfig brTag = new TagConfig(200, false);
            this.Add("br", brTag);

            TagConfig spanTag = new TagConfig(50);
            spanTag.CopyFrom(allStar);
            this.Add("span", spanTag);

            TagConfig h1Tag = new TagConfig(100);
            h1Tag.CopyFrom(allStar);
            this.Add("h1", h1Tag);

            TagConfig h2Tag = new TagConfig(100);
            h2Tag.CopyFrom(allStar);
            this.Add("h2", h2Tag);

            TagConfig h3Tag = new TagConfig(100);
            h3Tag.CopyFrom(allStar);
            this.Add("h3", h3Tag);



            //匹配表格
            TagConfig tableTag = new TagConfig(20);
            tableTag.CopyFrom(allStar);
            this.Add("table", tableTag);

            TagConfig tbodyTag = new TagConfig(20);
            tbodyTag.CopyFrom(allStar);
            this.Add("tbody", tbodyTag);

            TagConfig trTag = new TagConfig(20);
            trTag.CopyFrom(allStar);
            this.Add("tr", trTag);

            TagConfig tdTag = new TagConfig(20);
            tdTag.CopyFrom(allStar);
            this.Add("td", tdTag);
        }
        public static TagConfig CreateAllStyle()
        {
            TagConfig allStar = new TagConfig();
            allStar.Add("style", new Regex(@"^((font|color)\s*:\s*[^;]+;\s*)*$"));
            allStar.Add("class", new Regex(@"^[\w\s]*$"));
            allStar.Add("width", new Regex(@"^\d+%|\d+px$"));
            allStar.Add("height", new Regex(@"^\d+%|\d+px$"));
            return allStar;
        }
        /// <summary>
        /// 通过配置文件初始化
        /// </summary>
        /// <param name="root"></param>
        public FilterConfig(XmlElement root)
        {
            XmlNodeList xnl = root.GetElementsByTagName("tag");
            XmlElement wildcardElement = null;
            foreach (XmlElement xe in xnl)
            {
                if (xe.GetAttribute("name").Equals("*"))
                {
                    wildcardElement = xe;
                }
            }

            TagConfig wildcardConfig = wildcardElement == null ? null : new TagConfig(wildcardElement);

            foreach (XmlElement xe in xnl)
            {
                if (!xe.GetAttribute("name").Equals("*"))
                {
                    string name = xe.GetAttribute("name");
                    int maxCount = xe.HasAttribute("max") ? Core.TypeConvert.ToInt(xe.GetAttribute("max")) : 1;
                    TagConfig tagConfig = new TagConfig(xe);
                    tagConfig.MaxCount = maxCount;

                    foreach (KeyValuePair<string, Regex> pair in wildcardConfig)
                    {
                        if (!tagConfig.ContainsKey(pair.Key))
                        {
                            tagConfig.Add(pair.Key, pair.Value);
                        }
                    }

                    this.Add(name, tagConfig);
                }

            }
        }
    }
}
