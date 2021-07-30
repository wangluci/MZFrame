using MyAccess.Aop;
using MyAccess.DB.Attr;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 自动生成插入sql语句
    /// </summary>
    public class DoInsert : IDoSqlCommand
    {
        protected IDoSqlCommand mDo;
        protected object mInserted;
        public int RowCount
        {
            get
            {
                DoExecSql des = mDo as DoExecSql;
                if (des != null)
                {
                    return des.RowCount;
                }
                else
                {
                    return -1;
                }
            }
        }
        public DoQueryScalar LastQuery
        {
            get
            {
                return mDo as DoQueryScalar;
            }
        }

        /// <summary>
        /// 反射出要插入的数据
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public static void ObjToStr(object inserted, out string rtfields, out string rtvalues)
        {
            Type curObjType = inserted.GetType();
            rtfields = string.Empty;
            rtvalues = string.Empty;
            IBaseEntity be = inserted as IBaseEntity;
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
                bool caninserted = true;
                if (attrs.Length > 0)
                {
                    IDAttribute idattr = attrs[0] as IDAttribute;
                    caninserted = !idattr.IsAuto;
                }
                if (caninserted)
                {
                    rtfields += "," + pi.Name;
                    rtvalues += ",@" + pi.Name;
                }
            }

            if (rtfields.StartsWith(","))
            {
                rtfields = rtfields.Substring(1);
            }
            if (rtvalues.StartsWith(","))
            {
                rtvalues = rtvalues.Substring(1);
            }
        }

        public DoInsert(object inserted, string intosql, string middsql, string lastsql)
        {
            mInserted = inserted;
            string fields;
            string values;
            ObjToStr(inserted, out fields, out values);
            mDo = new DoExecSql(string.Format("{0}{1}{2}{3}{4}", intosql, fields, middsql, values, lastsql));
        }
        public DoInsert(object inserted, string tablename)
        {
            mInserted = inserted;
            string fields;
            string values;
            if (string.IsNullOrEmpty(tablename))
            {
                tablename = inserted.GetType().Name;
            }
            ObjToStr(inserted, out fields, out values);
            mDo = new DoExecSql(string.Format("insert into {0} ({1}) values ({2})", tablename, fields, values));
        }
        public DoInsert(object inserted, string tablename, string lastsql)
        {
            mInserted = inserted;
            string fields;
            string values;
            ObjToStr(inserted, out fields, out values);
            if (string.IsNullOrEmpty(tablename))
            {
                tablename = inserted.GetType().Name;
            }
            mDo = new DoQueryScalar(string.Format("insert into {0} ({1}) values ({2}){3}", tablename, fields, values, lastsql));
        }


        public void Excute(DbHelp help)
        {
            help.AddParamFrom(mInserted);
            mDo.Excute(help);
        }
        public async Task ExcuteAsync(DbHelp help)
        {
            help.AddParamFrom(mInserted);
            await mDo.ExcuteAsync(help);
        }

        public void SetSql(string sql)
        {
            mDo.SetSql(sql);
        }

        public string GetSql()
        {
            return mDo.GetSql();
        }
    }
}
