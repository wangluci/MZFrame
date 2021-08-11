using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 异步用DAL层
    /// </summary>
    public abstract class DBSupportAsync : IDBSupport
    {
        protected static AsyncLocal<IDbHelp> mDBHelp = new AsyncLocal<IDbHelp>();
        protected IDBFactory _dbFactory;
        public IDbHelp DBHelp
        {
            get { return mDBHelp.Value; }
        }

        public bool IsTranslation
        {
            get { return DBManAsync.Instance().IsTranslation; }
        }
        public DBSupportAsync(string connectionStr)
        {
            _dbFactory = CreateDBFactory(connectionStr);
        }
        public void InitHelp()
        {
            mDBHelp.Value = DBManAsync.Instance().OpenDB(_dbFactory);
        }

        protected abstract IDBFactory CreateDBFactory(string connstr);
    }
}
