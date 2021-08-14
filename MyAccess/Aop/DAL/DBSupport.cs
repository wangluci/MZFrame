using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// DAL层
    /// </summary>
    public abstract class DBSupport
    {
        protected static AsyncLocal<IDbHelp> mDBHelp = new AsyncLocal<IDbHelp>();
        protected string _connectionStr;

        public DBSupport(string connectionStr)
        {
            _connectionStr = connectionStr;
        }
        internal IDbHelp CreateHelp()
        {
            mDBHelp.Value = CreateDBHelpImp();
            return mDBHelp.Value;
        }
        protected abstract IDbHelp CreateDBHelpImp();

    }
}
