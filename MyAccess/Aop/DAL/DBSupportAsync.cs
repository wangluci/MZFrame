using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 异步用DAL层
    /// </summary>
    public abstract class DBSupportAsync : IDBFactory
    {
        protected static AsyncLocal<IDbHelp> mDBHelp = new AsyncLocal<IDbHelp>();
        protected string _connectionStr;
        public IDbHelp DBHelp
        {
            get { return mDBHelp.Value; }
        }

        public string Key
        {
            get { return _connectionStr; }
        }
        public DBSupportAsync(string connectionStr)
        {
            _connectionStr = connectionStr;
        }
        public void InitHelp()
        {
            mDBHelp.Value = DBManAsync.Instance().OpenDB(this);
        }

        public abstract IDbHelp CreateHelp();
    }
}
