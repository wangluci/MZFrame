using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 同步用DAL层
    /// </summary>
    public abstract class DBSupport
    {
        [ThreadStatic]
        protected static IDbHelp mDBHelp;

        public IDbHelp DBHelp
        {
            get { return mDBHelp; }
        }
        protected IDBConfig _config;
        public DBSupport(IDBConfig config)
        {
            _config = config;
        }
        public void InitHelp()
        {
            mDBHelp = DBMan.Instance().OpenDB(_config);
        }
    }
}
