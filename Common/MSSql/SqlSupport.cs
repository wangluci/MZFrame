using MyAccess.Aop.DAL;

namespace Common.MSSql
{
    /// <summary>
    /// MSSql同步操作用
    /// </summary>
    public abstract class SqlSupport : DBSupport
    {
        public SqlSupport(IDBConfig config) : base(config) { }
        protected SqlDbHelp help
        {
            get { return (SqlDbHelp)mDBHelp; }
        }
    }
}
