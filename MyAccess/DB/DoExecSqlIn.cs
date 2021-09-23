﻿using System;
using System.Collections.Generic;
using System.Text;
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
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mInArr.Length; i++)
            {
                sb.Append(",");
                sb.Append(help.AddParamAndReturn("inparam_" + i, mInArr[i]));
            }
            string inwhere = sb.ToString();
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
