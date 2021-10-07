using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Reflection;
using System.Text;
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
        private void ObjToStr(DbHelp help, ref string where, out string updatestr)
        {
            updatestr = "";
            StringBuilder sb = new StringBuilder();
            bool autowhere = string.IsNullOrEmpty(where);
            Type curObjType = mUpdated.GetType();

            IBaseEntity be = mUpdated as IBaseEntity;
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
                if (pi.IsDefined(typeof(DataIgnoreAttribute)))
                {
                    continue;
                }
                object[] attrs = pi.GetCustomAttributes(typeof(IDAttribute), false);
                bool canupdated = true;
                if (attrs.Length > 0)
                {
                    canupdated = false;
                    if (autowhere)
                    {
                        where = pi.Name + "=" + help.AddParamAndReturn(pi.Name, pi.GetValue(mUpdated));
                    }
                }
                if (canupdated)
                {
                    sb.Append(",");
                    sb.Append(pi.Name);
                    sb.Append("=");
                    sb.Append(help.AddParamAndReturn(pi.Name, pi.GetValue(mUpdated)));
                }
            }
            updatestr = sb.ToString();
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
            else
            {
                _tablename = tablename;
            }
        }
        public DoUpdate(object updated, string tablename) : this(updated, tablename, "") { }
        private void ExcuteInit(DbHelp help)
        {
            string updatestr;
            //重设sql语句
            ObjToStr(help, ref mwherestr, out updatestr);
            string rtSQL = "update " + _tablename + " set ";
            if (string.IsNullOrEmpty(mwherestr))
            {
                rtSQL += updatestr;
            }
            else
            {
                rtSQL += updatestr + " where " + mwherestr;
            }
            SetSql(rtSQL);
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
