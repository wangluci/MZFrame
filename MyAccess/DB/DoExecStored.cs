using MyAccess.Core;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行指定存储过程
    /// </summary>
    public class DoExecStored : IDoCommand
    {
        protected string mName;
        protected int mRowCount;

        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }
        public DoExecStored(string name)
        {
            mName = name;
            mRowCount = -1;

        }
        public virtual void Excute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = mName;

            foreach (DbParameter p in help.DbParamters)
            {
                if (!command.Parameters.Contains(p.ParameterName))
                {
                    command.Parameters.Add(p);
                }
            }

            mRowCount = command.ExecuteNonQuery();
            help.ClearParams();
        }

        public virtual async Task ExcuteAsync(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = mName;

            foreach (DbParameter p in help.DbParamters)
            {
                if (!command.Parameters.Contains(p.ParameterName))
                {
                    command.Parameters.Add(p);
                }
            }

            mRowCount = await command.ExecuteNonQueryAsync();
            help.ClearParams();
        }
    }
}
