using System;
using System.Data;
namespace MyAccess.DB
{
    public class DoQueryAllTable : IDoCommand
    {
        private DataTable mSchema;
        public DataTable Schema
        {
            get { return mSchema; }
        }
        public void Excute(DbHelp help)
        {
            mSchema = help.Connection.GetSchema("Tables");
        }
    }
}
