using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 自动生成插入sql语句
    /// </summary>
    public class DoInsert<T> : DoExecSql where T : class
    {
        protected IDoSqlCommand mDo;
        protected T[] _inserted;
        protected string _tablename;



        public DoInsert(T inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = new T[1];
            _inserted[0] = inserted;
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
            else
            {
                _tablename = tablename;
            }
        }
        public DoInsert(T[] inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = inserted;
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
            else
            {
                _tablename = tablename;
            }
        }
        public DoInsert(List<T> inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = inserted.ToArray();
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
            else
            {
                _tablename = tablename;
            }
        }
        /// <summary>
        /// 反射出要插入的数据
        /// </summary>
        /// <param name="inserted"></param>
        /// <param name="tablename"></param>
        /// <param name="rtfields"></param>
        /// <param name="rtvalues"></param>
        private void ObjToStr(DbHelp help, out string rtfields, out string rtvalues)
        {
            rtfields = string.Empty;
            rtvalues = string.Empty;
            StringBuilder sbfields = new StringBuilder();
            StringBuilder sbvalues = new StringBuilder();

            if (_inserted == null || _inserted.Length == 0) return;
            Type curObjType = InterceptFactory.GetProxyType(_inserted[0]);


            PropertyInfo[] myProInfos = curObjType.GetProperties();

            for (int idx = 0; idx < _inserted.Length; idx++)
            {
                T iitem = _inserted[idx];
                sbvalues.Append(",(");
                bool isfirst = true;
                for (int i = 0; i < myProInfos.Length; i++)
                {
                    PropertyInfo pi = myProInfos[i];
                    IDAttribute idattr = (IDAttribute)pi.GetCustomAttribute(typeof(IDAttribute), false);
                    bool caninserted = true;
                    if (idattr != null)
                    {
                        caninserted = !idattr.IsAuto;
                    }
                    if (caninserted)
                    {
                        if (idx == 0)
                        {
                            sbfields.Append(",");
                            sbfields.Append(pi.Name);
                        }
                        if (isfirst)
                        {
                            isfirst = false;
                            sbvalues.Append(help.AddParamAndReturn("iptparam_" + idx + "_" + i, pi.GetValue(iitem)));
                        }
                        else
                        {
                            sbvalues.Append(",");
                            sbvalues.Append(help.AddParamAndReturn("iptparam_" + idx + "_" + i, pi.GetValue(iitem)));
                        }
                    }
                }
                sbvalues.Append(")");
            }

            rtfields = sbfields.ToString();
            if (rtfields.StartsWith(","))
            {
                rtfields = rtfields.Substring(1);
            }
            rtfields = "(" + rtfields + ")";
            rtvalues = sbvalues.ToString();
            if (rtvalues.StartsWith(","))
            {
                rtvalues = rtvalues.Substring(1);
            }

        }

        private void ExcuteInit(DbHelp help)
        {
            if (string.IsNullOrEmpty(mSqlText))
            {
                string fields;
                string values;
                ObjToStr(help, out fields, out values);
                mSqlText = string.Format("insert into {0} {1} values {2}", _tablename, fields, values);
            }

        }
        public override void Excute(DbHelp help)
        {
            ExcuteInit(help);
            base.Excute(help);
        }
        public override async Task ExcuteAsync(DbHelp help)
        {
            ExcuteInit(help);
            await base.ExcuteAsync(help);
        }

    }
}
