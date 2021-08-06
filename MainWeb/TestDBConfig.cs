using Common.MySql;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Configuration;

namespace MainWeb
{
    public class TestDBConfig : IDBConfig
    {
        private string _connstr;
        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
        }
        public TestDBConfig()
        {
            _connstr = ConfigurationManager.AppSettings["TestDBConn"];
            _key = string.Format("MySql://{0}", _connstr);
        }
        public IDbHelp CreateHelp()
        {
            return new MySqlHelp(_connstr);
        }
    }
}