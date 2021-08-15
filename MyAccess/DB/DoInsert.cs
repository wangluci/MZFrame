using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 自动生成插入sql语句
    /// </summary>
    public class DoInsert<T> : DoExecSql where T:class
    {
        protected IDoSqlCommand mDo;
        protected T[] _inserted;
        protected string _tablename;

        /// <summary>
        /// 反射出要插入的数据
        /// </summary>
        /// <param name="inserted"></param>
        /// <param name="tablename"></param>
        /// <param name="rtfields"></param>
        /// <param name="rtvalues"></param>
        public static bool ObjToStr(DbHelp help, T[] iptObjs, out string rtfields, out string rtvalues)
        {
            rtfields = string.Empty;
            rtvalues = string.Empty;
            if (iptObjs == null || iptObjs.Length == 0) return false;
            Type curObjType = iptObjs[0].GetType();


            PropertyInfo[] myProInfos = curObjType.GetProperties();

            for (int idx = 0; idx < iptObjs.Length; idx++)
            {
                T iitem = iptObjs[idx];
                IBaseEntity be = iitem as IBaseEntity;
                rtvalues += ",(";
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
                        if (i == 0)
                        {
                            rtfields += "," + pi.Name;
                        }
                        rtvalues += "," + help.AddParamAndReturn("inparam_" + idx, iitem);
                    }
                }
                rtvalues += ")";
            }


            if (rtfields.StartsWith(","))
            {
                rtfields = rtfields.Substring(1);
            }
            rtfields = "(" + rtfields + ")";
            if (rtvalues.StartsWith(","))
            {
                rtvalues = rtvalues.Substring(1);
            }
            return true;
        }

        public DoInsert(T inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = new T[1];
            _inserted[0] = inserted;
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
        }
        public DoInsert(T[] inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = inserted;
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
        }
        public DoInsert(List<T> inserted, string tablename = "") : base(string.Empty)
        {
            _inserted = inserted.ToArray();
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(inserted);
            }
        }


        private void ExcuteInit(DbHelp help)
        {
            if (string.IsNullOrEmpty(mSqlText))
            {
                string fields;
                string values;
                ObjToStr(help, _inserted, out fields, out values);
                mSqlText = string.Format("insert into {0} {1} values ({2})", _tablename, fields, values);
            }

        }
        public void Excute(DbHelp help)
        {
            ExcuteInit(help);
            base.Excute(help);
        }
        public async Task ExcuteAsync(DbHelp help)
        {
            ExcuteInit(help);
            await base.ExcuteAsync(help);
        }

    }
}
