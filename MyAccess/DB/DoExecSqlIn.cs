using System;
using System.Collections.Generic;

namespace MyAccess.DB
{
    public class DoExecSqlIn<T> : DoExecSql
    {
        protected T[] mInArr;
        protected string mParamName;
        public DoExecSqlIn(List<T> inlist, string paramname, string sql) : base(sql)
        {
            mInArr = inlist.ToArray();
            mParamName = paramname;
        }
        public DoExecSqlIn(T[] inarr, string paramname, string sql) : base(sql)
        {
            mInArr = inarr;
            mParamName = paramname;
        }
        public override void Excute(DbHelp help)
        {
            string inwhere = string.Empty;

            for (int i = 0; i < mInArr.Length; i++)
            {
                string tmpn = "@inparam_" + i;
                help.AddParam(tmpn, mInArr[i]);
                inwhere += "," + tmpn;
            }
            if (inwhere.StartsWith(","))
            {
                inwhere = inwhere.Substring(1);
            }
            mSqlText = mSqlText.Replace(mParamName, inwhere);
            base.Excute(help);
        }
    }
}
