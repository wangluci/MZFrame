using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 多实体返回查询
    /// </summary>
    public class DoQueryGroup : IDoCommand
    {
        private IDoQuerySql[] _querys;
        private string _sep;
        public DoQueryGroup(params IDoQuerySql[] querys)
        {
            _querys = querys;
            _sep = ";";
        }
        public DoQueryGroup(string sep, params IDoQuerySql[] querys)
        {
            _querys = querys;
            _sep = sep;
        }
        private DbCommand PreExcute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            StringBuilder sb = new StringBuilder();
            foreach (IDoQuerySql q in _querys)
            {
                q.GenerateSql(help);
                sb.Append(q.GetSql());
                sb.Append(_sep);
            }
            command.CommandText = sb.ToString();
            help.InitParamters(command);
            return command;
        }
        public void Excute(DbHelp help)
        {
            DbCommand command = PreExcute(help);
            DbDataReader dataReader = command.ExecuteReader();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _querys.Length > i)
                {
                    while (dataReader.Read())
                    {
                        _querys[i].SetResult(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
        }

        public async Task ExcuteAsync(DbHelp help)
        {
            DbCommand command = PreExcute(help);
            DbDataReader dataReader = await command.ExecuteReaderAsync();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _querys.Length > i)
                {
                    while (await dataReader.ReadAsync())
                    {
                        await _querys[i].SetResultAsync(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
        }
    }
}
