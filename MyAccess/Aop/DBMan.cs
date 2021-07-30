using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace MyAccess.Aop
{
    /// <summary>
    /// 同步DBHelp管理器
    /// </summary>
    public class DBMan
    {
        private volatile static DBMan mInstance = null;
        private static readonly object lockHelper = new object();

        private IDBStore _store;
        private DBMan() {
            _store = new ThreadDBStore();
        }
        public static DBMan Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                        mInstance = new DBMan();
                }
            }
            return mInstance;
        }
        public bool IsTranslation
        {
            get { return _store.IsTranslation(); }
        }

        public IDbHelp OpenDB(IDBConfig dbconfig)
        {
            return _store.OpenDB(dbconfig);
        }
        public void BeginTrans()
        {
            _store.BeginTrans(Isolation.DEFAULT);
        }
        public void BeginTrans(Isolation level)
        {
            _store.BeginTrans(level);
        }
        public void Commit()
        {
            _store.Commit();
        }
        public void RollBack()
        {
            _store.RollBack();
        }

    }
}
