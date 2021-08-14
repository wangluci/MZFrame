using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MySql
{
    /// <summary>
    /// MySql异步操作用
    /// </summary>
    public abstract class MySqlSupport : DBSupport
    {
        public MySqlSupport(string connectionStr) : base(connectionStr) { }
        protected MySqlHelp help
        {
            get { return (MySqlHelp)mDBHelp.Value; }
        }
        protected override IDbHelp CreateDBHelpImp()
        {
            return new MySqlHelp(_connectionStr);
        }
    }
}
