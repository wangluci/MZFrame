using MyAccess.Aop.DAL;
namespace Common.Oracle
{
    public abstract class OracleSupport : DBSupport
    {
        public OracleSupport(IDBConfig config) : base(config) { }
        protected OracleHelp help
        {
            get { return (OracleHelp)mDBHelp; }
        }
    }
}
