using MyAccess.Aop.DAL;
using MyAccess.DB;


namespace MyAccess.Aop
{
    /// <summary>
    /// 异步DBHelp管理器
    /// </summary>
    public class AsyncDBMan
    {
        private volatile static AsyncDBMan mInstance = null;
        private static readonly object lockHelper = new object();

        private IDBStore _store;
        private AsyncDBMan()
        {
            _store = new AsyncDBStore();
        }
        public static AsyncDBMan Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                        mInstance = new AsyncDBMan();
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
