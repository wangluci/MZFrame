using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MSSql
{
    /// <summary>
    /// MSSql异步操作用
    /// </summary>
    public abstract class SqlSupportAsync : DBSupportAsync
    {
        public SqlSupportAsync(string connectionStr) : base(connectionStr) { }
        protected SqlDbHelp help
        {
            get { return (SqlDbHelp)mDBHelp.Value; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new MSSqlDBFactory(connstr);
        }
    }
}
