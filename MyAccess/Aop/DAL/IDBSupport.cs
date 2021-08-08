using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    public interface IDBSupport
    {
        bool IsTranslation { get; }
        IDbHelp DBHelp { get; }
        void InitHelp();
    }
}
