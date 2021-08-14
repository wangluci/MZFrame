using System;
using System.Data.Common;

using MyAccess.DB;
using System.Data;
using System.Data.OracleClient;
namespace Common.Oracle
{
    public class OracleHelp : DbHelp
    {
        public OracleHelp(string connstr) : base(connstr) { }
        public OracleHelp(string host,int port, string userName, string password, string dbName) :
            base(string.Format(@"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))
	   (CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = {4}) ) );User ID={2};PassWord={3};", host, port, userName, password, dbName))
        { }
        public override DbCommand CreateCommand()
        {
#pragma warning disable CS0618 // “OracleCommand”已过时:“OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
            return new OracleCommand();
#pragma warning restore CS0618 // “OracleCommand”已过时:“OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
        }

        public override DbConnection CreateConnection()
        {
#pragma warning disable CS0618 // “OracleConnection”已过时:“OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
            return new OracleConnection();
#pragma warning restore CS0618 // “OracleConnection”已过时:“OracleConnection has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
        }

        public override DbDataAdapter CreateDataAdapter()
        {
#pragma warning disable CS0618 // “OracleDataAdapter”已过时:“OracleDataAdapter has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
            return new OracleDataAdapter();
#pragma warning restore CS0618 // “OracleDataAdapter”已过时:“OracleDataAdapter has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260”
        }


        protected override void AutoDbParam(Type tp, string name, object val, ParameterDirection direct)
        {
            OracleParameter dbParameter = new OracleParameter(name, val);
            dbParameter.Direction = direct;
            mDbParamters.Add(dbParameter);
        }


        public void CopyMySqlParamFrom(OracleParameter[] parameters)
        {
            CopyDbParamFrom(parameters);
        }

        public void AddOutParam(string parameterName, OracleType dbType)
        {
            OracleParameter dbParameter = new OracleParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            mDbParamters.Add(dbParameter);
        }
        public void AddInParam(string parameterName, OracleType dbType, object value)
        {
            OracleParameter dbParameter = new OracleParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            mDbParamters.Add(dbParameter);
        }
        protected override string NameToDbParam(string param)
        {
            return ":" + param;
        }
    }
}
