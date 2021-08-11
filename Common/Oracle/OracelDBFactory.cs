using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
namespace Common.Oracle
{
    public class OracelDBFactory : IDBFactory
    {
        private string _connstr;
        public OracelDBFactory(string connstr)
        {
            _connstr = connstr;
        }
        public string Key
        {
            get { return _connstr; }
        }

        public IDbHelp CreateHelp()
        {
            return new OracleHelp(_connstr);
        }
    }
}
