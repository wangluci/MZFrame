using MyAccess.DB;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace Common.MSSql
{
    public class SqlDbHelp : DbHelp
    {
        public SqlDbHelp(string connstr) : base(connstr) { }
        public SqlDbHelp(string host, string userName, string password, string dbName) :
            base(string.Format("Max Pool Size = 2048;Server={0};UID={1};PWD={2};DataBase={3};Pooling=true;", host, userName, password, dbName))
        { }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }
        public override DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// 自带清除之前的参数
        /// </summary>
        /// <param name="parameters"></param>
        public void CopySqlParamFrom(SqlParameter[] parameters)
        {
            CopyDbParamFrom(parameters);
        }
        protected override void AutoDbParam(Type tp, string name, object val, ParameterDirection direct)
        {
            SqlParameter dbParameter = new SqlParameter();
            dbParameter.ParameterName = name;
            dbParameter.Value = val;
            dbParameter.Direction = direct;

            if (tp == typeof(string))
            {
                dbParameter.SqlDbType = SqlDbType.VarChar;
            }
            else if (tp == typeof(int))
            {
                dbParameter.SqlDbType = SqlDbType.Int;
            }
            else if (tp == typeof(long))
            {
                dbParameter.SqlDbType = SqlDbType.BigInt;
            }
            else if (tp == typeof(float) || tp == typeof(double))
            {
                dbParameter.SqlDbType = SqlDbType.Float;
            }
            else if (tp == typeof(Int16))
            {
                dbParameter.SqlDbType = SqlDbType.SmallInt;
            }
            else if (tp == typeof(bool))
            {
                dbParameter.SqlDbType = SqlDbType.Bit;
            }
            else if (tp == typeof(decimal))
            {
                dbParameter.SqlDbType = SqlDbType.Decimal;
            }
            else if (tp == typeof(DateTime))
            {
                dbParameter.SqlDbType = SqlDbType.DateTime;
            }
            else if (tp.IsEnum)
            {
                dbParameter.SqlDbType = SqlDbType.SmallInt;
            }
            else
            {
                dbParameter.SqlDbType = SqlDbType.VarChar;
            }
            mDbParamters.Add(dbParameter);
        }

        public void AddOutParam(string parameterName, SqlDbType dbType)
        {
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            mDbParamters.Add(dbParameter);
        }
        public void AddInParam(string parameterName, SqlDbType dbType, object value)
        {
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
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