using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.Oracle
{
    public abstract class OracleSupport : DBSupport
    {
        public OracleSupport(string connectionStr) : base(connectionStr) { }
        protected OracleHelp help
        {
            get { return (OracleHelp)mDBHelp; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new OracelDBFactory(connstr);
        }
    }
}
