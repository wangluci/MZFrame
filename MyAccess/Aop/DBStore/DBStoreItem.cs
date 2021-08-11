using MyAccess.Aop.DAL;
using MyAccess.DB;
using System.Collections.Generic;

namespace MyAccess.Aop
{
    public class DBStoreItem
    {
        protected bool _openTrans = false;
        protected Isolation _level = Isolation.DEFAULT;
        protected Dictionary<string, IDbHelp> mHelpStoreList = new Dictionary<string, IDbHelp>();
        public virtual bool IsTranslation()
        {
            return _openTrans;
        }
        public virtual IDbHelp OpenDB(IDBFactory factory)
        {
            IDbHelp tdb;
            if (!mHelpStoreList.TryGetValue(factory.Key, out tdb))
            {
                tdb = factory.CreateHelp();
                mHelpStoreList.Add(factory.Key, tdb);
            }
            if (_openTrans)
            {
                if (tdb.IsNoTran())
                {
                    tdb.BeginTran(_level);
                }
            }
            return tdb;
        }
        public virtual void BeginTrans(Isolation level)
        {
            _level = level;
            _openTrans = true;
            mHelpStoreList.Clear();
        }
        public virtual void Commit()
        {
            if (_openTrans)
            {
                _openTrans = false;
                foreach (KeyValuePair<string, IDbHelp> item in mHelpStoreList)
                {
                    IDbHelp tdb = item.Value;
                    if (!Equals(tdb, null) && !tdb.IsNoTran())
                    {
                        tdb.Commit();
                    }
                }
            }
        }
        public virtual void RollBack()
        {
            if (_openTrans)
            {
                _openTrans = false;
                foreach (KeyValuePair<string, IDbHelp> item in mHelpStoreList)
                {
                    IDbHelp tdb = item.Value;
                    if (!Equals(tdb, null) && !tdb.IsNoTran())
                    {
                        tdb.RollBack();
                    }
                }
            }
        }
    }
}
