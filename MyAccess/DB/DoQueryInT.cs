using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行In查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoQueryInT<T,Z> : DoQuerySql<Z>
    {
        protected T[] mInArr;
        protected string mParamName;
        public DoQueryInT(List<T> inlist, string paramname, string sql) : base(sql)
        {
            mInArr = inlist.ToArray();
            mParamName = paramname;
        }
        public DoQueryInT(T[] inarr, string paramname, string sql) : base(sql)
        {
            mInArr = inarr;
            mParamName = paramname;
        }
 
        public override void GenerateSql(DbHelp help)
        {
            string inwhere = string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mInArr.Length; i++)
            {
                sb.Append(",");
                sb.Append(help.AddParamAndReturn("inparam_" + i, mInArr[i]));
            }
            inwhere = sb.ToString();
            if (inwhere.StartsWith(","))
            {
                inwhere = inwhere.Substring(1);
            }
            ReplaceSql(mParamName, inwhere);
        }

    }
}
