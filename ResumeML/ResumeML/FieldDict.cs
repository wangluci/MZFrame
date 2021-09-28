using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeML
{
    public class FieldDict
    {
        public static string[] FieldNameArrary = { "##", "籍贯", "身高", "体重", "婚姻状况", "政治面貌", "专业类别", "毕业时间", "毕业学校", "专业", "学制", "学校", "教育经历", "工作经验", "基本资料" };
        public static string[] DiplomaField = { "初中", "高中", "中技", "中专", "大专", "本科", "硕士", "博士", "小学" };
        private HashSet<string> _sets;
        private HashSet<string> _dipsets;
        public FieldDict()
        {
            _sets = new HashSet<string>();
            foreach(string s in FieldNameArrary)
            {
                _sets.Add(s);
            }
            _dipsets = new HashSet<string>();
            foreach(string s in DiplomaField)
            {
                _dipsets.Add(s);
            }
        }
        public bool IsFieldName(string k)
        {
            return _sets.Contains(k);
        }
        public bool IsDiploma(string s)
        {
            return _dipsets.Contains(s);
        }
    }
}
