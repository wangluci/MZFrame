using MyAccess.Aop.DAL;

namespace Common.MySql
{
    /// <summary>
    /// MySql异步操作用
    /// </summary>
    public abstract class MySqlSupportAsync : DBSupportAsync
    {
        public MySqlSupportAsync(IDBConfig config) : base(config) { }
        protected MySqlHelp help
        {
            get { return (MySqlHelp)mDBHelp.Value; }
        }
    }
}
