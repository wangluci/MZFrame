using System;

namespace MyAccess.DB
{
    public interface IDoSqlCommand: IDoCommand
    {
        void SetSql(string sql);
        string GetSql();
    }
}
