using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class DoQuerySql<T> : QueryResult<T>, IDoQuerySql
    {
        private string mSql;

        public void SetSql(string sql)
        {
            mSql = sql;
        }
        public string GetSql()
        {
            return mSql;
        }
        public string ReplaceSql(string oldValue, string newValue)
        {
            mSql = mSql.Replace(oldValue, newValue);
            return mSql;
        }

        public DoQuerySql(string sql)
        {
            mSql = sql;
        }

        protected virtual DbCommand PreExcute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = mSql;
            help.InitParamters(command);
            return command;
        }
        public virtual void Excute(DbHelp help)
        {
            DbCommand command = PreExcute(help);
            DbDataReader dataReader = command.ExecuteReader();
            using (dataReader)
            {
                bool drbl = true;
                while (drbl)
                {
                    while (dataReader.Read())
                    {
                        SetResult(dataReader);
                    }
                    drbl = dataReader.NextResult();
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
                while (drbl)
                {
                    while (await dataReader.ReadAsync())
                    {
                        await SetResultAsync(dataReader);
                    }
                    drbl = dataReader.NextResult();
                }
            }
        }

 
        public virtual void GenerateSql(DbHelp help) { }

    }
}
