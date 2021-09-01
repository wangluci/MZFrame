using PanGu;
using ResumeML.Business;
using ResumeMLML.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TemplateAction.Core;
using Microsoft.Extensions.ML;
using System.Linq;

namespace ResumeML
{
    public class Home : TABaseController
    {
        private TestBusiness _business;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
        public Home(TestBusiness business, PredictionEnginePool<ModelInput, ModelOutput> prediction)
        {
            _predictionEnginePool = prediction;
            _business = business;
        }
        public ViewResult Test()
        {
            return View();
        }
        /// <summary>
        /// 业务逻辑调用测试
        /// </summary>
        /// <returns></returns>
        public TextResult TestBus()
        {
            return Content(_business.printTest());
        }
        /// <summary>
        /// 文字段落转为简历
        /// </summary>
        /// <param name="paragraphdata"></param>
        /// <returns></returns>
        public AjaxResult Index(string paragraphdata = "")
        {
            List<string> datalist = null;
            try
            {
                paragraphdata = Context.UrlDecode(paragraphdata, Encoding.UTF8);
                datalist = MyAccess.Json.Json.DecodeType<List<string>>(paragraphdata);
            }
            catch
            {
                return Err("传入参数格式错误");
            }

            AIResume resume = new AIResume();
            resume.Gender = -1;
            resume.Diploma = -1;
            resume.Appraise = string.Empty;
            resume.WorkList = new List<WorkItem>();
            resume.EduList = new List<EduItem>();
            List<string> EduSchool = new List<string>();
            List<int> EduDiploma = new List<int>();
            try
            {
                LinkedList<string> newlist = new LinkedList<string>();
                //合并单个字的
                for (int i = datalist.Count - 1; i >= 0; i--)
                {
                    if (datalist[i].Length <= 1)
                    {
                        string wstr = datalist[i];
                        i = contactPreSingleWord(ref wstr, i, datalist);
                        newlist.AddFirst(wstr);
                    }
                    else
                    {
                        newlist.AddFirst(datalist[i]);
                    }
                }
                LinkedList<string> sparalist = new LinkedList<string>();
                //字段分隔
                LinkedListNode<string> linkNodeString = newlist.First;
                bool canappendmark = true;
                while (linkNodeString != null)
                {
                    string tmps = linkNodeString.Value;
                    linkNodeString = linkNodeString.Next;
                    if (!FieldDict.Instance.IsFieldName(tmps))
                    {
                        PanGu.Segment segment = new PanGu.Segment();
                        ICollection<WordInfo> words = segment.DoSegment(tmps);
                        StringBuilder segments = new StringBuilder();
                        foreach (WordInfo w in words)
                        {
                            segments.AppendFormat("{0} ", w.Word);
                        }
                        string colfield = segments.ToString().Trim();
                        //机器学习算法匹配类型
                        ModelInput sampleData = new ModelInput()
                        {
                            Col1 = colfield,
                        };
                        ModelOutput predictionResult = _predictionEnginePool.Predict("ResumeModel", sampleData);
                        float preScore = predictionResult.Score.Max();
                        if (predictionResult.Prediction == 0)
                        {
                            if (preScore > 0.4 && isChinese(tmps))
                            {
                                if (string.IsNullOrEmpty(resume.TrueName))
                                {
                                    resume.TrueName = tmps;
                                }
                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 1)
                        {
                            if (preScore > 0.6)
                            {
                                resume.Appraise += tmps;
                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 2)
                        {
                            if (preScore > 0.6)
                            {
                                if (string.IsNullOrEmpty(resume.Address))
                                {
                                    resume.Address = tmps;
                                }
                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 3)
                        {
                            if (preScore > 0.6)
                            {
                                WorkItem item = null;
                                if (resume.WorkList.Count == 0)
                                {
                                    item = new WorkItem();
                                    resume.WorkList.Add(item);
                                    canappendmark = true;
                                }
                                else
                                {
                                    item = resume.WorkList[resume.WorkList.Count - 1];
                                    if (string.IsNullOrEmpty(item.Remark))
                                    {
                                        canappendmark = true;
                                    }
                                    else
                                    {
                                        if (!canappendmark)
                                        {
                                            item = new WorkItem();
                                            resume.WorkList.Add(item);
                                            canappendmark = true;
                                        }
                                    }
                                }
                                if (item.Remark == null)
                                {
                                    item.Remark = tmps;
                                }
                                else
                                {
                                    item.Remark += tmps;
                                }

                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 4)
                        {
                            if (preScore > 0.5)
                            {
                                WorkItem item = null;
                                if (resume.WorkList.Count == 0)
                                {
                                    item = new WorkItem();
                                    resume.WorkList.Add(item);
                                }
                                else
                                {
                                    WorkItem wi = resume.WorkList[resume.WorkList.Count - 1];
                                    if (string.IsNullOrEmpty(wi.JobName))
                                    {
                                        item = wi;
                                    }
                                    else
                                    {
                                        item = new WorkItem();
                                        resume.WorkList.Add(item);
                                    }
                                }
                                if (tmps.StartsWith("职位名称") || tmps.StartsWith("工作职位"))
                                {
                                    tmps = tmps.Substring(4);
                                }
                                item.JobName = tmps;
                                canappendmark = false;
                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 5)
                        {
                            if (preScore > 0.5)
                            {
                                WorkItem item = null;
                                if (resume.WorkList.Count == 0)
                                {
                                    item = new WorkItem();
                                    resume.WorkList.Add(item);
                                }
                                else
                                {
                                    WorkItem wi = resume.WorkList[resume.WorkList.Count - 1];
                                    if (string.IsNullOrEmpty(wi.CompanyName))
                                    {
                                        item = wi;
                                    }
                                    else
                                    {
                                        item = new WorkItem();
                                        resume.WorkList.Add(item);
                                    }
                                }
                                if (tmps.StartsWith("所在单位") || tmps.StartsWith("公司名称"))
                                {
                                    tmps = tmps.Substring(4);
                                }
                                item.CompanyName = tmps;
                                canappendmark = false;
                                continue;
                            }
                        }
                        else if (predictionResult.Prediction == 6)
                        {
                            if (preScore > 0.5)
                            {
                                if (tmps.StartsWith("学校名称"))
                                {
                                    tmps = tmps.Substring(4);
                                }
                                EduSchool.Add(tmps);
                                continue;
                            }
                        }


                    }
                    //包含字段的处理
                    tmps = tmps.Replace("姓名", "##:0");
                    tmps = tmps.Replace("性别", "##:1");
                    tmps = tmps.Replace("出生年月", "##:2").Replace("出生日期", "##:2").Replace("生日", "##:2");
                    tmps = tmps.Replace("现所在地", "##:3");
                    tmps = tmps.Replace("学历", "##:4");
                    tmps = tmps.Replace("求职意向", "##:5").Replace("求职岗位", "##:5");
                    string[] tarr = tmps.Split(FieldDict.FieldNameArrary, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ss in tarr)
                    {
                        sparalist.AddLast(ss);
                    }

                }

                //判断是否为字段类型说明文字
                foreach (string s in sparalist)
                {
                    if (s.StartsWith(":0"))
                    {
                        if (string.IsNullOrEmpty(resume.TrueName))
                        {
                            resume.TrueName = ClearSpace(s.Substring(2));
                        }
                        continue;
                    }


                    if (resume.Gender == -1)
                    {
                        if (s.StartsWith(":1"))
                        {
                            string tmpss = s.Substring(2);
                            if (tmpss.Contains("男"))
                            {
                                resume.Gender = 1;
                            }
                            else if (tmpss.Contains("女"))
                            {
                                resume.Gender = 0;
                            }
                            continue;
                        }
                    }

                    if (s.StartsWith(":5"))
                    {
                        if (string.IsNullOrEmpty(resume.HopePost))
                        {
                            resume.HopePost = ClearSpace(s.Substring(2));
                        }
                        continue;
                    }



                    if (s.StartsWith(":4"))
                    {
                        string tmpsss = s.Substring(2);
                        int tmpdip = ParseDiploma(tmpsss);
                        if (resume.Diploma == -1)
                        {
                            resume.Diploma = tmpdip;
                        }
                        else
                        {
                            EduDiploma.Add(tmpdip);
                        }
                        continue;
                    }

                    if (FieldDict.Instance.IsDiploma(s))
                    {
                        int tmpdip = ParseDiploma(s);
                        if (resume.Diploma == -1)
                        {
                            resume.Diploma = tmpdip;
                        }
                        bool hasdip = false;
                        foreach (int sdip in EduDiploma)
                        {
                            if (sdip == tmpdip)
                            {
                                hasdip = true;
                                break;
                            }
                        }
                        if (!hasdip)
                        {
                            EduDiploma.Add(tmpdip);
                        }
                        continue;
                    }

                    if (resume.Gender == -1)
                    {
                        if (s == "男")
                        {
                            resume.Gender = 1;
                            continue;
                        }
                        else if (s == "女")
                        {
                            resume.Gender = 0;
                            continue;
                        }
                    }
                }

                //生成教育经历
                if (EduSchool.Count > 0)
                {
                    for (int i = 0; i < EduSchool.Count; i++)
                    {
                        EduItem tmpitem = new EduItem();
                        tmpitem.SchoolName = EduSchool[i];
                        if (i < EduDiploma.Count)
                        {
                            tmpitem.Diploma = EduDiploma[i];
                        }
                        resume.EduList.Add(tmpitem);
                    }
                }
            }
            catch (Exception ex)
            {
                return Err(ex.Message);
            }
            return Success(MyAccess.Json.Json.EncodeType<AIResume>(resume));
        }
        /// <summary>
        /// 清除空格
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ClearSpace(string input)
        {
            return Regex.Replace(input, @"[\s:：]*", "");
        }
        private int ParseDiploma(string s)
        {
            for (int i = 0; i < FieldDict.DiplomaField.Length; i++)
            {
                string df = FieldDict.DiplomaField[i];
                if (s.Contains(df))
                {
                    if (i == 8)
                    {
                        return 11999;
                    }
                    else
                    {
                        return 11000 + i + 1;
                    }
                }
            }
            return 11999;
        }
        private bool isChinese(string inp)
        {
            bool result;
            if (inp.Length > 6)
            {
                result = false;
            }
            else
            {
                string text = "李王张刘陈杨赵黄周吴徐孙胡朱林何郭马罗梁宋郑谢韩唐冯于董萧曹袁邓许傅沈曾彭吕苏卢蒋蔡贾丁魏薛叶阎余潘杜戴夏汪任姜范姚谭廖邹熊陆郝孔崔邱秦史侯邵孟段章钱汤尹黎易贺赖龚文龙施吕尤卫褚";
                for (int i = 0; i < text.Length; i++)
                {
                    if (inp[0] == text[i])
                    {
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            return result;
        }
        private bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        /// <summary>
        /// 验证QQ
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        private bool IsValidQQ(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[1-9]([0-9]{5,11})$");
        }
        /// <summary>
        /// 验证手机
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        private bool IsValidMobile(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(13|14|15|16|17|18|19)[0-9]{9}$");
        }
        /// <summary>
        /// 合并单字
        /// </summary>
        /// <param name="w"></param>
        /// <param name="i"></param>
        /// <param name="slist"></param>
        /// <returns></returns>
        private int contactPreSingleWord(ref string w, int i, List<string> slist)
        {
            int tmppreidx = i - 1;
            if (tmppreidx >= 0)
            {
                if (slist[tmppreidx].Length <= 1)
                {
                    i = tmppreidx;
                    w = string.Concat(slist[tmppreidx], w);
                    return contactPreSingleWord(ref w, i, slist);
                }
            }
            return i;
        }
    }
}
