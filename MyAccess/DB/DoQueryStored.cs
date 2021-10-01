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
        protected override DbCommand PreExcute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = mSql;
            foreach (DbParameter p in help.DbParamters)
            {
                if (!command.Parameters.Contains(p.ParameterName))
                {
                    command.Parameters.Add(p);
                }
            }
            return command;

        }
        protected override void AfterExcute(DbCommand command)
        {
            foreach (DbParameter dbp in command.Parameters)
            {
                if (dbp.Direction == ParameterDirection.InputOutput || dbp.Direction == ParameterDirection.Output)
                {
                    mOutDict.Add(dbp.ParameterName, dbp.Value);
                }
            }
        }

    }
}
