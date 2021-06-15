using System;
using System.Data;

namespace MyAccess.DB
{
    public class DoExecAndReturnStored<T> : DoExecStored
    {
        private object _returnVal;
        public DoExecAndReturnStored(string name) : base(name)
        {
        }
        public T ReturnValue
        {
            get { return (T)_returnVal; }
        }
        public override void Excute(DbHelp help)
        {
            help.Command.CommandType = CommandType.StoredProcedure;
            help.Command.CommandText = mName;
            help.AddReturnParam(typeof(T), "@ReturnValue");
            help.InitParams();
            mRowCount = help.Command.ExecuteNonQuery();
            IDataParameter dataParameter = help.Command.Parameters["@ReturnValue"];
            _returnVal = dataParameter.Value;
            help.ClearParams();
        }
    }
}
