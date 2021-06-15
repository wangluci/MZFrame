using System;

namespace MyAccess.DB
{
    public class DoQueryCount : IDoSqlCommand
    {
        private string cmdSql;
        private int mRecordCount;
        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RecordCount
        {
            get { return mRecordCount; }
        }
        public DoQueryCount(string table, string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                cmdSql = "select count(*) from " + table + " where " + where;
            }
            else
            {
                cmdSql = "select count(*) from " + table;
            }
        }
        public void Excute(DbHelp help)
        {
            DoQueryScalar query = new DoQueryScalar(cmdSql);
            query.Excute(help);
            mRecordCount = query.GetValueInt(0);
         }

        public void SetSql(string sql)
        {
            cmdSql = sql;
        }
        public string GetSql()
        {
            return cmdSql;
        }
    }
}
