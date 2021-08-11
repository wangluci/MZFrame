using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.Oracle
{
    public abstract class OracleSupportAsync : DBSupportAsync
    {
        public OracleSupportAsync(string connectionStr) : base(connectionStr) { }
        protected OracleHelp help
        {
            get { return (OracleHelp)mDBHelp.Value; }
        }
        protected override IDBFactory CreateDBFactory(string connstr)
        {
            return new OracelDBFactory(connstr);
        }
    }
}
