using System;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoSqlCommand: IDoCommand
    {
        void SetSql(string sql);
        string GetSql();
        string ReplaceSql(string oldValue, string newValue);
    }
}
