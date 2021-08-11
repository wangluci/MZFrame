using MyAccess.Aop.DAL;
using MyAccess.DB;
namespace Common.MSSql
{
    public class MSSqlDBFactory : IDBFactory
    {
        private string _connstr;
        public MSSqlDBFactory(string connstr)
        {
            _connstr = connstr;
        }
        public string Key
        {
            get { return _connstr; }
        }

        public IDbHelp CreateHelp()
        {
            return new SqlDbHelp(_connstr);
        }
    }
}
