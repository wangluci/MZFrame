using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;

namespace MyAccess.Aop
{
    public interface IDBStore
    {
        /// <summary>
        /// 是否已启用事务
        /// </summary>
        bool IsTranslation();
        IDbHelp OpenDB(IDBFactory factory);
        void BeginTrans(Isolation level);
        void Commit();
        void RollBack();
    }
}
