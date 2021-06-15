using System;
using System.Data;
namespace MyAccess.DB
{
    /// <summary>
    /// 执行删除、更改时使用，返回被影响记录数
    /// </summary>
    public class DoExecSql : IDoSqlCommand
    {
        protected string mSqlText;
        protected int mRowCount;
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

        public virtual void Excute(DbHelp help)
        {
            help.Command.CommandType = CommandType.Text;
            help.Command.CommandText = mSqlText;
            help.InitParams();
            mRowCount = help.Command.ExecuteNonQuery();
            help.ClearParams();
        }

        public string GetSql()
        {
            return mSqlText;
        }
    }
}
