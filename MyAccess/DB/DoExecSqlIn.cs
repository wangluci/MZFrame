using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 批量条件更新、删除
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        private void ExcuteDoExecSqlInInit(DbHelp help)
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
            mSqlText = mSqlText.Replace(mParamName, inwhere);
        }
        public override void Excute(DbHelp help)
        {
            ExcuteDoExecSqlInInit(help);
            base.Excute(help);
        }
        public override async Task ExcuteAsync(DbHelp help)
        {
            ExcuteDoExecSqlInInit(help);
            await base.ExcuteAsync(help);
        }
    }
}
