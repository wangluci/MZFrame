using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;

namespace MyAccess.Aop
{
    public class ThreadDBStore : IDBStore
    {
        [ThreadStatic]
        private static DBStoreItem _storeItem;
        public bool IsTranslation()
        {
            if (_storeItem != null)
            {
                return _storeItem.IsTranslation();
            }
            else
            {
                return false;
            }
        }
        public IDbHelp OpenDB(IDBConfig dbconfig)
        {
            if (_storeItem == null)
            {
                _storeItem = new DBStoreItem();
            }
            return _storeItem.OpenDB(dbconfig);
        }
        public void BeginTrans(Isolation level)
        {
            if (_storeItem != null)
            {
                _storeItem.BeginTrans(level);
            }
        }
        public void Commit()
        {
            if (_storeItem != null)
            {
                _storeItem.Commit();
            }
        }
        public void RollBack()
        {
            if (_storeItem != null)
            {
                _storeItem.RollBack();
            }
        }
    }
}
