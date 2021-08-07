using MyAccess.DB;
using System;

namespace MyAccess.Aop.DAL
{
    public interface IDBFactory
    {
        string Key { get; }
        IDbHelp CreateHelp();
    }
}
