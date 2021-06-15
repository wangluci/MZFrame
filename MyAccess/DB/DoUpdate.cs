using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Reflection;

namespace MyAccess.DB
{
    public class DoUpdate : DoExecSql
    {
        protected object mUpdated;
        /// <summary>
        /// 反射出要插入的数据
        /// </summary>
        /// <param name="updated"></param>
        /// <returns></returns>
        public static void ObjToStr(object updated, ref string where, out string updatestr)
        {
            updatestr = "";
            bool autowhere = string.IsNullOrEmpty(where);
            Type curObjType = updated.GetType();

            IBaseEntity be = updated as IBaseEntity;
            PropertyInfo[] myProInfos;
            if (be != null)
            {
                myProInfos = be.GetUsedPropertys();
            }
            else
            {
                myProInfos = curObjType.GetProperties();
            }
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                object[] attrs = pi.GetCustomAttributes(typeof(IDAttribute), false);
                bool canupdated = true;
                if (attrs.Length > 0)
                {
                    canupdated = false;
                    if (autowhere)
                    {
                        where = pi.Name + "=@" + pi.Name;
                    }
                }
                if (canupdated)
                {
                    updatestr += "," + pi.Name + "=@" + pi.Name;
                }
            }
            if (updatestr.StartsWith(","))
            {
                updatestr = updatestr.Substring(1);
            }
        }
        public DoUpdate(object updated, string tablename, string where) : base("")
        {
            mUpdated = updated;
            string updatestr;
            string wherestr = where;
            //重设sql语句
            ObjToStr(updated, ref wherestr, out updatestr);
            if (string.IsNullOrEmpty(tablename))
            {
                tablename = updated.GetType().Name;
            }
            string rtSQL = "update " + tablename + " set ";
            if (string.IsNullOrEmpty(wherestr))
            {
                rtSQL += updatestr;
            }
            else
            {
                rtSQL += updatestr + " where " + wherestr;
            }
            SetSql(rtSQL);
        }
        public DoUpdate(object updated, string tablename) : this(updated, tablename, "") { }
        public override void Excute(DbHelp help)
        {
            help.AddParamFrom(mUpdated);
            base.Excute(help);
        }
    }
}
