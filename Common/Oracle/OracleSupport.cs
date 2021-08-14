using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.Oracle
{
    public abstract class OracleSupport : DBSupport
    {
        public OracleSupport(string connectionStr) : base(connectionStr) { }
        protected OracleHelp help
        {
            get { return (OracleHelp)mDBHelp.Value; }
        }
        protected override IDbHelp CreateDBHelpImp()
        {
            return new OracleHelp(_connectionStr);
        }
    }
}
