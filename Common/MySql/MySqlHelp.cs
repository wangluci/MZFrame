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
            MySqlParameter dbParameter = new MySqlParameter();
            dbParameter.ParameterName = name;
            dbParameter.Value = val;
            dbParameter.Direction = direct;
            if (tp == typeof(string))
            {
                dbParameter.MySqlDbType = MySqlDbType.VarChar;
            }
            else if (tp == typeof(int))
            {
                dbParameter.MySqlDbType = MySqlDbType.Int32;
            }
            else if (tp == typeof(long))
            {
                dbParameter.MySqlDbType = MySqlDbType.Int64;
            }
            else if (tp == typeof(float))
            {
                dbParameter.MySqlDbType = MySqlDbType.Float;
            }
            else if (tp == typeof(double))
            {
                dbParameter.MySqlDbType = MySqlDbType.Double;
            }
            else if (tp == typeof(Int16))
            {
                dbParameter.MySqlDbType = MySqlDbType.Int16;
            }
            else if (tp == typeof(bool))
            {
                dbParameter.MySqlDbType = MySqlDbType.Bit;
            }
            else if (tp == typeof(decimal))
            {
                dbParameter.MySqlDbType = MySqlDbType.Decimal;
            }
            else if (tp == typeof(DateTime))
            {
                dbParameter.MySqlDbType = MySqlDbType.DateTime;
            }
            else if (tp == typeof(byte))
            {
                dbParameter.MySqlDbType = MySqlDbType.UByte;
            }
            else if (tp.IsEnum)
            {
                dbParameter.MySqlDbType = MySqlDbType.Int16;
            }
            else
            {
                dbParameter.MySqlDbType = MySqlDbType.VarString;
            }
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
