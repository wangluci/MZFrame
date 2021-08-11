using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MSSql
{
    /// <summary>
    /// MSSql同步操作用
    /// </summary>
    public abstract class SqlSupport : DBSupport
    {
        public SqlSupport(string connectionStr) : base(connectionStr) { }
        protected SqlDbHelp help
        {
            get { return (SqlDbHelp)mDBHelp; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new MSSqlDBFactory(connstr);
        }
    }
}
