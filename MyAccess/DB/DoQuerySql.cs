using System.Data;
using System.Data.Common;

namespace MyAccess.DB
{
    public class DoQuerySql : DoQueryBase
    {
        public DoQuerySql(string sql)
        {
            mSql = sql;
        }

        public override void Excute(DbHelp help)
        {
            help.Command.CommandType = CommandType.Text;
            help.Command.CommandText = mSql;
            help.InitParams();
            DbDataAdapter adapter = help.CreateDataAdapter();
            adapter.SelectCommand = help.Command;
            adapter.Fill(mData);
            help.ClearParams();
        }
    }
}
