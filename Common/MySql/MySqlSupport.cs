using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MySql
{
    /// <summary>
    /// MySql同步操作用
    /// </summary>
    public abstract class MySqlSupport : DBSupport
    {
        public MySqlSupport(string connectionStr) : base(connectionStr) { }
        protected MySqlHelp help
        {
            get { return (MySqlHelp)mDBHelp; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new MySqlDBFactory(connstr);
        }
    }
}
