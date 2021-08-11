using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    public abstract class DBSupportBase
    {
        internal abstract IDbHelp DBHelp { get; }

        internal abstract bool IsTranslation { get; }
        internal abstract void InitHelp();
    }
}
