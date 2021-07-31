using MyAccess.Aop.DAL;

namespace Common.MSSql
{
    /// <summary>
    /// MSSql异步操作用
    /// </summary>
    public abstract class SqlSupportAsync : DBSupportAsync
    {
        public SqlSupportAsync(IDBConfig config) : base(config) { }
        protected SqlDbHelp help
        {
            get { return (SqlDbHelp)mDBHelp.Value; }
        }
    }
}
