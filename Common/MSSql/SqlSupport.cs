using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MSSql
{
    /// <summary>
    /// MSSql异步操作用
    /// </summary>
    public abstract class SqlSupport : DBSupport
    {
        public SqlSupport(string connectionStr) : base(connectionStr) { }
        protected SqlDbHelp help
        {
            get { return (SqlDbHelp)mDBHelp.Value; }
        }
        protected override IDbHelp CreateDBHelpImp()
        {
            return new SqlDbHelp(_connectionStr);
        }
    }
}
