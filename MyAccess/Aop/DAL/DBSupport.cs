using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 同步用DAL层
    /// </summary>
    public abstract class DBSupport : DBSupportBase
    {
        [ThreadStatic]
        protected static IDbHelp mDBHelp;
        protected IDBFactory _dbFactory;
     
        public DBSupport(string connectionStr)
        {
            _dbFactory = CreateDBFactory(connectionStr);
        }

        internal override IDbHelp DBHelp
        {
            get { return mDBHelp; }
        }

        internal override bool IsTranslation
        {
            get { return DBMan.Instance().IsTranslation; }
        }
        internal override void InitHelp()
        {
            mDBHelp = DBMan.Instance().OpenDB(_dbFactory);
        }

        protected abstract IDBFactory CreateDBFactory(string connstr);
    }
}
