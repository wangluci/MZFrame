using MyAccess.Aop.DAL;


namespace Common.Oracle
{
    public abstract class OracleSupportAsync : DBSupportAsync
    {
        public OracleSupportAsync(IDBConfig config) : base(config) { }
        protected OracleHelp help
        {
            get { return (OracleHelp)mDBHelp.Value; }
        }
    }
}
