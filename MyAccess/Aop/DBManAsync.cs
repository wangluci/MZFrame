using MyAccess.Aop.DAL;
using MyAccess.DB;


namespace MyAccess.Aop
{
    /// <summary>
    /// 异步DBHelp管理器
    /// </summary>
    public class DBManAsync
    {
        private volatile static DBManAsync mInstance = null;
        private static readonly object lockHelper = new object();

        private IDBStore _store;
        private DBManAsync()
        {
            _store = new DBStoreAsync();
        }
        public static DBManAsync Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                        mInstance = new DBManAsync();
                }
            }
            return mInstance;
        }
        public bool IsTranslation
        {
            get { return _store.IsTranslation(); }
        }

        public IDbHelp OpenDB(IDBFactory factory)
        {
            return _store.OpenDB(factory);
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
