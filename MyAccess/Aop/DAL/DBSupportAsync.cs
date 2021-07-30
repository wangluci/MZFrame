using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 异步用DAL层
    /// </summary>
    public abstract class DBSupportAsync
    {
        protected static AsyncLocal<IDbHelp> mDBHelp;

        public IDbHelp DBHelp
        {
            get { return mDBHelp.Value; }
        }
        protected IDBConfig _config;
        public DBSupportAsync(IDBConfig config)
        {
            _config = config;
        }
        public void InitHelp()
        {
            //mDBHelp = DBMan.Instance().OpenDB(_config);
        }
    }
}
