using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 异步用DAL层
    /// </summary>
    public abstract class DBSupportAsync : DBSupportBase
    {
        protected static AsyncLocal<IDbHelp> mDBHelp = new AsyncLocal<IDbHelp>();
        protected IDBFactory _dbFactory;

        public DBSupportAsync(string connectionStr)
        {
            _dbFactory = CreateDBFactory(connectionStr);
        }
        internal override IDbHelp DBHelp
        {
            get { return mDBHelp.Value; }
        }

        internal override bool IsTranslation
        {
            get { return DBManAsync.Instance().IsTranslation; }
        }
        internal override void InitHelp()
        {
            mDBHelp.Value = DBManAsync.Instance().OpenDB(_dbFactory);
        }

        protected abstract IDBFactory CreateDBFactory(string connstr);
    }
}
