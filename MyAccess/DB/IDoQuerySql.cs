using System;
using System.Data.Common;

namespace MyAccess.DB
{
    public interface IDoQuerySql : IDoSqlCommand, IDoResult<DbDataReader>
    {
        void GenerateSql(DbHelp help);
    }
}
