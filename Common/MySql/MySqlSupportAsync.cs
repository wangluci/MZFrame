using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MySql
{
    /// <summary>
    /// MySql异步操作用
    /// </summary>
    public abstract class MySqlSupportAsync : DBSupportAsync
    {
        public MySqlSupportAsync(string connectionStr) : base(connectionStr) { }
        protected MySqlHelp help
        {
            get { return (MySqlHelp)mDBHelp.Value; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new MySqlDBFactory(connstr);
        }
    }
}
