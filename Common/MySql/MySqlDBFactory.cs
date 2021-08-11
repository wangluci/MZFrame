using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace Common.MySql
{
    public class MySqlDBFactory : IDBFactory
    {
        private string _connstr;
        public MySqlDBFactory(string connstr)
        {
            _connstr = connstr;
        }
        public string Key
        {
            get { return _connstr; }
        }

        public IDbHelp CreateHelp()
        {
            return new MySqlHelp(_connstr);
        }
    }
}
