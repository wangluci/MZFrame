using System;
using System.Data;
using System.Data.Common;
namespace MyAccess.DB
{
    public class DoQueryReader : IDoSqlCommand
    {
        private string mSql;
        private DbDataReader mDataReader;

        public DbDataReader Reader
        {
            get { return mDataReader; }
        }
        public DoQueryReader(int intCount, string fieldName, string tableName)
            : this(intCount, fieldName, tableName, "", "")
        {
        }
        public DoQueryReader(int intCount, string fieldName, string tableName, string where)
            : this(intCount, fieldName, tableName, where, "")
        {
        }
        public DoQueryReader(int intCount, string fieldName, string tableName, string where, string order)
        {
            string tCount = "";
            if (intCount > 0)
            {
                tCount = " top " + intCount.ToString();
            }
            if ((fieldName == null) || (fieldName.Trim() == ""))
            {
                fieldName = " * ";
            }
            if ((where == null) || (where.Trim() == ""))
            {
                where = " ";
            }
            else
            {
                where = " where " + where;
            }
            if ((order == null) || (order.Trim() == ""))
            {
                order = " ";
            }
            else
            {
                order = " order by " + order;
            }

            mSql = "select " + tCount + " " + fieldName + " from " + tableName + where + order;
        }
        public DoQueryReader(string sql)
        {
            mSql = sql;
        }
        public void Excute(DbHelp help)
        {
            help.Command.CommandType = CommandType.Text;
            help.Command.CommandText = mSql;
            help.InitParams();
            mDataReader = help.Command.ExecuteReader();
            help.ClearParams();
            
        }

        public void SetSql(string sql)
        {
            mSql = sql;
        }
        public string GetSql()
        {
            return mSql;
        }
    }
}
