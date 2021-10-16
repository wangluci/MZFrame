using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 存储过程多个返回实体
    /// </summary>
    public class DoQueryStoredGroup : IDoCommand
    {
        private IDoResult<DbDataReader>[] _results;
        private string mName;
        private Dictionary<string, object> mOutDict;
        public DoQueryStoredGroup(string name, params IDoResult<DbDataReader>[] results)
        {
            _results = results;
            mName = name;
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
        private DbCommand PreExcute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = mName;
            help.InitParamters(command);
            return command;

        }
        private void AfterExcute(DbCommand command)
        {
            foreach (DbParameter dbp in command.Parameters)
            {
                if (dbp.Direction == ParameterDirection.InputOutput || dbp.Direction == ParameterDirection.Output)
                {
                    mOutDict.Add(dbp.ParameterName, dbp.Value);
                }
            }
        }

        public void Excute(DbHelp help)
        {
            DbCommand command = PreExcute(help);
            DbDataReader dataReader = command.ExecuteReader();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _results.Length > i)
                {
                    while (dataReader.Read())
                    {
                        _results[i].SetResult(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
            AfterExcute(command);
        }

        public async Task ExcuteAsync(DbHelp help)
        {
            DbCommand command = PreExcute(help);
            DbDataReader dataReader = await command.ExecuteReaderAsync();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _results.Length > i)
                {
                    while (await dataReader.ReadAsync())
                    {
                        await _results[i].SetResultAsync(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
            AfterExcute(command);
        }
    }
}
