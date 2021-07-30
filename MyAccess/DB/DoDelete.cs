using System;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class DoDelete : IDoSqlCommand
    {
        private string cmdTxt;
        private int mRowCount;
        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }
        public DoDelete(string table, string where)
        {
            cmdTxt = "delete from " + table + " where " + where; ;
        }
        public void Excute(DbHelp help)
        {
            DoExecSql exSql = new DoExecSql(cmdTxt);
            exSql.Excute(help);
            mRowCount = exSql.RowCount;
        }
        public async Task ExcuteAsync(DbHelp help)
        {
            DoExecSql exSql = new DoExecSql(cmdTxt);
            await exSql.ExcuteAsync(help);
            mRowCount = exSql.RowCount;
        }
        public void SetSql(string sql)
        {
            cmdTxt = sql;
        }

        public string GetSql()
        {
            return cmdTxt;
        }

    
    }
}
