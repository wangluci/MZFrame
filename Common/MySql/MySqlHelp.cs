using System;
using System.Data.Common;
using MyAccess.DB;
using System.Data;
using MySqlConnector;

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
        protected override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        protected override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }


        protected override void AutoDbParam(string name, object val, ParameterDirection direct)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
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
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            mDbParamters.Add(dbParameter);
        }
        public void AddInParam(string parameterName, MySqlDbType dbType, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            mDbParamters.Add(dbParameter);
        }

        protected override string NameToDbParam(string param)
        {
            if (param.StartsWith("@"))
            {
                return param;
            }
            else
            {
                return "@" + param;
            }
        }
    }
}
