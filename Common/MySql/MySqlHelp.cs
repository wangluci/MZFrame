using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MyAccess.DB;
using System.Data;

namespace Common.MySql
{
    /// <summary>
    /// mysql数据库访问类
    /// </summary>
    public class MySqlHelp : DbHelp
    {
        public MySqlHelp(string connstr) : base(connstr) { }
        public MySqlHelp(string host, string userName, string password, string dbName) :
            base(string.Format("server={0};user id={1};password={2};database={3};", host, userName, password, dbName))
        { }
        public override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }


        protected override void AutoDbParam(Type tp, string name, object val, ParameterDirection direct)
        {
            MySqlParameter dbParameter = new MySqlParameter(name, val);
            dbParameter.Direction = direct;
            mDbParamters.Add(dbParameter);
        }


        public void CopyMySqlParamFrom(MySqlParameter[] parameters)
        {
            CopyDbParamFrom(parameters);
        }

        public void AddOutParam(string parameterName, MySqlDbType dbType)
        {
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            mDbParamters.Add(dbParameter);
        }
        public void AddInParam(string parameterName, MySqlDbType dbType, object value)
        {
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            mDbParamters.Add(dbParameter);
        }
        protected override string NameToDbParam(string param)
        {
            return "@" + param;
        }
    }
}
