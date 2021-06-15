using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeML
{
    public class AIResume
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// 所在地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 求职意向
        /// </summary>
        public string HopePost { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public int Diploma { get; set; }
        /// <summary>
        /// 1为男,0为女
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 自我评价
        /// </summary>
        public string Appraise { get; set; }
        public List<WorkItem> WorkList { get; set; }
        public List<EduItem> EduList { get; set; }
    }
}
