using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 更新数据命令
    /// </summary>
    public class DoUpdate : DoExecSql
    {
        private object mUpdated;
        private string mwherestr;
        private string _tablename;
        /// <summary>
        /// 反射出要插入的数据
        /// </summary>
        /// <param name="updated"></param>
        /// <returns></returns>
        private void ObjToStr(DbHelp help, object updated, ref string where, out string updatestr)
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
                        where = pi.Name + "=" + help.AddParamAndReturn(pi.Name, pi.GetValue(updated));
                    }
                }
                if (canupdated)
                {
                    updatestr += "," + pi.Name + "=" + help.AddParamAndReturn(pi.Name, pi.GetValue(updated));
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
            mwherestr = where;
            if (string.IsNullOrEmpty(tablename))
            {
                _tablename = InterceptFactory.GetProxyTypeName(updated);
            }
        }
        public DoUpdate(object updated, string tablename) : this(updated, tablename, "") { }
        private void ExcuteInit(DbHelp help)
        {
            string updatestr;
            //重设sql语句
            ObjToStr(help, mUpdated, ref mwherestr, out updatestr);
            string rtSQL = "update " + _tablename + " set ";
            if (string.IsNullOrEmpty(mwherestr))
            {
                rtSQL += updatestr;
            }
            else
            {
                rtSQL += updatestr + " where " + mwherestr;
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
