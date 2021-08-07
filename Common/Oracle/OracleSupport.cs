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
        public override IDbHelp CreateHelp()
        {
            return new OracleHelp(this._connectionStr);
        }
    }
}
