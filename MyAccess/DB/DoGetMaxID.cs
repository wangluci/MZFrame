using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace MyAccess.DB
{
    public class DoGetMaxID
    {
        private string mField;
        private string mTable;
        private int mMaxID;
        public int MaxID
        {
            get { return mMaxID; }
        }
        public DoGetMaxID(string field, string table)
        {
            mField = field;
            mTable = table;
        }
        public void Excute(DbHelp help)
        {
            DoQueryScalar query = new DoQueryScalar("select max(" + mField + ") as MaxID from " + mTable);
            query.Excute(help);
            mMaxID = query.GetValueInt(0);
        }
    }
}
