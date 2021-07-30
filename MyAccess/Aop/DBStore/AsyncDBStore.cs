using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop
{
    public class AsyncDBStore : IDBStore
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
        public IDbHelp OpenDB(IDBConfig dbconfig)
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi == null)
            {
                dbsi = new DBStoreItem();
                _storeItem.Value = dbsi;
            }
            return dbsi.OpenDB(dbconfig);
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
