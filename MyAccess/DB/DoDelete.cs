using System;

namespace MyAccess.DB
{
    public class DoDelete : DoExecSql
    {
        public DoDelete(string table, string where) : base("delete from " + table + " where " + where)
        {
        }
    }
}
