using System;
using System.Data;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行删除、更改时使用，返回被影响记录数
    /// </summary>
    public class DoExecSql : IDoSqlCommand
    {
        protected string mSqlText;
        protected int mRowCount;
        protected IDoSqlCommand mDoCommand;
        /// <summary>
        /// 替换执行的IDoSqlCommand
        /// </summary>
        /// <param name="command"></param>
        public void UseSqlCommand(IDoSqlCommand command)
        {
            mDoCommand = command;
        }
        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }

        public DoExecSql(string sql)
        {
            mDoCommand = null;
            mSqlText = sql;
            mRowCount = -1;
        }
        public void SetSql(string sql)
        {
            mSqlText = sql;
        }

        public virtual void Excute(DbHelp help)
        {
            if (mDoCommand != null)
            {
                mDoCommand.Excute(help);
            }
            else
            {
                help.Command.CommandType = CommandType.Text;
                help.Command.CommandText = mSqlText;
                help.InitParams();
                mRowCount = help.Command.ExecuteNonQuery();
                help.ClearParams();
            }

        }

        public virtual async Task ExcuteAsync(DbHelp help)
        {
            if (mDoCommand != null)
            {
                await mDoCommand.ExcuteAsync(help);
            }
            else
            {
                help.Command.CommandType = CommandType.Text;
                help.Command.CommandText = mSqlText;
                help.InitParams();
                mRowCount = await help.Command.ExecuteNonQueryAsync();
                help.ClearParams();
            }
        }

        public string GetSql()
        {
            return mSqlText;
        }
    }
}
