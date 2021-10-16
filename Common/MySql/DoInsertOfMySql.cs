using MyAccess.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Common.MySql
{
    public class DoInsertOfMySql<T> : DoInsert<T> where T : class
    {
        public long LastInsertedId { get; set; }
        public DoInsertOfMySql(T inserted, string tablename = "") : base(inserted, tablename)
        {
        }
        public DoInsertOfMySql(T[] inserted, string tablename = "") : base(inserted, tablename)
        {
        }
        public DoInsertOfMySql(List<T> inserted, string tablename = "") : base(inserted, tablename)
        {
        }

        protected override void AfterExcute(DbCommand command)
        {
            LastInsertedId = ((MySqlCommand)command).LastInsertedId;
        }
    }
}
