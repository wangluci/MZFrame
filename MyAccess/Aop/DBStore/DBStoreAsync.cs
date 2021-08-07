using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop
{
    public class DBStoreAsync : IDBStore
    {
        private static AsyncLocal<DBStoreItem> _storeItem = new AsyncLocal<DBStoreItem>();
        public bool IsTranslation()
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi != null)
            {
                return dbsi.IsTranslation();
            }
            else
            {
                return false;
            }
        }
        public IDbHelp OpenDB(IDBFactory factory)
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi == null)
            {
                dbsi = new DBStoreItem();
                _storeItem.Value = dbsi;
            }
            return dbsi.OpenDB(factory);
        }
        public void BeginTrans(Isolation level)
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi != null)
            {
                dbsi.BeginTrans(level);
            }
        }
        public void Commit()
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi != null)
            {
                dbsi.Commit();
            }
        }
        public void RollBack()
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi != null)
            {
                dbsi.RollBack();
            }
        }
    }
}
