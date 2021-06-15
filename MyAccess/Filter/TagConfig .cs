using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
namespace MyAccess.Filter
{
    public class TagConfig : Dictionary<string, Regex>
    {
        /// <summary>
        /// 统计指定属性个数
        /// </summary>
        private Dictionary<string, int> mAttrNumbers = new Dictionary<string, int>();
        public int GetAttrCount(string attr)
        {
            int count = 0;
            if (mAttrNumbers.TryGetValue(attr, out count))
            {
                return count;
            }
            return 0;
        }
        private int mTagCount;
        public int TagCount
        {
            get { return mTagCount; }
        }

        private int mMaxCount;
        public int MaxCount
        {
            get { return mMaxCount; }
            set { mMaxCount = value; }
        }
        private int mNeedCloseNum;
        /// <summary>
        /// 需要闭合的标签数
        /// </summary>
        public int NeedCloseNum
        {
            get
            {
                if (mNeedClose)
                {
                    return mNeedCloseNum;
                }
                else
                {
                    return 0;
                }
            }
            set { mNeedCloseNum = value; }
        }
        /// <summary>
        /// 重置计数
        /// </summary>
        public void ResetCount()
        {
            mTagCount = 0;
        }
        /// <summary>
        /// 累加
        /// </summary>
        public void Increase()
        {
            mTagCount++;
        }
        /// <summary>
        /// 增加属性数
        /// </summary>
        /// <param name="attr"></param>
        public void IncreaseAttrCount(string attr)
        {
            if (mAttrNumbers.ContainsKey(attr))
            {
                mAttrNumbers[attr]++;
            }
            else
            {
                mAttrNumbers[attr] = 0;
            }
        }
        private bool mNeedClose;
        public TagConfig(int max = 0, bool needclose = true)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            mMaxCount = max;
            mNeedClose = needclose;
        }

        public TagConfig(XmlElement tagNode)
        {
            XmlNodeList xnlist = tagNode.GetElementsByTagName("attr");
            foreach (XmlElement ele in xnlist)
            {
                this.Add(ele.GetAttribute("name"), new Regex(ele.Value));
            }
        }
        public void CopyFrom(TagConfig tag)
        {
            foreach (KeyValuePair<string, Regex> pair in tag)
            {
                this.Add(pair.Key, pair.Value);
            }
        }
    }
}
