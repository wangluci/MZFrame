using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行删除、更改时使用，返回被影响记录数
    /// </summary>
    public class DoExecSql : IDoSqlCommand
    {
        private string mSqlText;
        private int mRowCount;
        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }

        public DoExecSql(string sql)
        {
            mSqlText = sql;
            mRowCount = -1;
        }
        public void SetSql(string sql)
        {
            mSqlText = sql;
        }
        public string GetSql()
        {
            return mSqlText;
        }
        public string ReplaceSql(string oldValue, string newValue)
        {
            mSqlText = mSqlText.Replace(oldValue, newValue);
            return mSqlText;
        }
        protected virtual void AfterExcute(DbCommand command) { }
        public virtual void GenerateSql(DbHelp help) { }
        public virtual void Excute(DbHelp help)
        {
            GenerateSql(help);
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = mSqlText;

            help.InitParamters(command);
            mRowCount = command.ExecuteNonQuery();
            AfterExcute(command);
        }

        public virtual async Task ExcuteAsync(DbHelp help)
        {
            GenerateSql(help);
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = mSqlText;
            help.InitParamters(command);
            mRowCount = await command.ExecuteNonQueryAsync();
            AfterExcute(command);
        }


    }
}
