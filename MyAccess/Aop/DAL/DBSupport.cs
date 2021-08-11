using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 同步用DAL层
    /// </summary>
    public abstract class DBSupport : IDBSupport
    {
        [ThreadStatic]
        protected static IDbHelp mDBHelp;
        protected IDBFactory _dbFactory;
        public IDbHelp DBHelp
        {
            get { return mDBHelp; }
        }


        public bool IsTranslation
        {
            get { return DBMan.Instance().IsTranslation; }
        }

        public DBSupport(string connectionStr)
        {
            _dbFactory = CreateDBFactory(connectionStr);
        }

        public void InitHelp()
        {
            mDBHelp = DBMan.Instance().OpenDB(_dbFactory);
        }

        protected abstract IDBFactory CreateDBFactory(string connstr);
    }
}
