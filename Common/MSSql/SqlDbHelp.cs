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
        /// 拷贝多个参数
        /// </summary>
        /// <param name="parameters"></param>
        public void CopySqlParamFrom(SqlParameter[] parameters)
        {
            AddParamFromArray(parameters);
        }
        protected override void AutoDbParam(string name, object val, ParameterDirection direct)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
            SqlParameter dbParameter = new SqlParameter(name, val);
            dbParameter.Direction = direct;
            AddParam(dbParameter);
        }

        public void AddOutParam(string parameterName, SqlDbType dbType)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            AddParam(dbParameter);
        }
        public void AddInParam(string parameterName, SqlDbType dbType, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            AddParam(dbParameter);
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