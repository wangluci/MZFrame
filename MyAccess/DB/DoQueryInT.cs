using System;
using System.Collections.Generic;
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
        private void InitDoQueryInT(DbHelp help)
        {
            string inwhere = string.Empty;

            for (int i = 0; i < mInArr.Length; i++)
            {
                string tmpn = help.AddParam("inparam_" + i, mInArr[i]);
                inwhere += "," + tmpn;
            }
            if (inwhere.StartsWith(","))
            {
                inwhere = inwhere.Substring(1);
            }
            mSql = mSql.Replace(mParamName, inwhere);
        }
        public override void Excute(DbHelp help)
        {
            InitDoQueryInT(help);
            base.Excute(help);
        }
        public override async Task ExcuteAsync(DbHelp help)
        {
            InitDoQueryInT(help);
            await base.ExcuteAsync(help);
        }
    }
}
