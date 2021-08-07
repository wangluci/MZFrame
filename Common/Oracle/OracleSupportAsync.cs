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
        public override IDbHelp CreateHelp()
        {
            return new OracleHelp(this._connectionStr);
        }
    }
}
