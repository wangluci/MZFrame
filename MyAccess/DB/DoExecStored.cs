using MyAccess.Core;
using System;
using System.Data;
namespace MyAccess.DB
{
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
            help.Command.CommandType = CommandType.StoredProcedure;
            help.Command.CommandText = mName;
            help.InitParams();
            mRowCount = help.Command.ExecuteNonQuery();
            help.ClearParams();
        }
    }
}
