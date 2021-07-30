using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace MyAccess.DB
{
    public class DoQueryStored<T> : DoQuerySql<T>
    {
        private Dictionary<string, object> mOutDict;
        public DoQueryStored(string name) : base(name)
        {
            mOutDict = new Dictionary<string, object>();
        }
        public int OutInt(string key)
        {
            if (mOutDict.ContainsKey(key))
            {
                string inputval = mOutDict[key].ToString();
                int rt = 0;
                if (int.TryParse(inputval, out rt))
                {
                    return rt;
                }
            }
            return 0;
        }
        public string OutStr(string key)
        {
            if (mOutDict.ContainsKey(key))
            {
                return mOutDict[key].ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return mOutDict[key]; }
        }
        protected override void PreExcute(DbHelp help)
        {
            help.Command.CommandType = CommandType.StoredProcedure;
            help.Command.CommandText = mSql;
            help.InitParams();
        }
        protected override void AfterExcute(DbHelp help)
        {
            foreach (DbParameter dbp in help.Command.Parameters)
            {
                if (dbp.Direction == ParameterDirection.InputOutput || dbp.Direction == ParameterDirection.Output)
                {
                    mOutDict.Add(dbp.ParameterName, dbp.Value);
                }
            }
            base.AfterExcute(help);
        }

    }
}
