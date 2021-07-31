using MyAccess.Aop.DAL;

namespace Common.MySql
{
    /// <summary>
    /// MySql同步操作用
    /// </summary>
    public abstract class MySqlSupport : DBSupport
    {
        public MySqlSupport(IDBConfig config) : base(config) { }
        protected MySqlHelp help
        {
            get { return (MySqlHelp)mDBHelp; }
        }
    }
}
