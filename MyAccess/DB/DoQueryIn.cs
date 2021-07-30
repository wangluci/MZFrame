using System;
using System.Collections.Generic;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行in
    /// </summary>
    public class DoQueryIn<T> : DoQueryInT<string,T>
    {
        public DoQueryIn(List<string> inlist, string paramname, string sql) : base(inlist, paramname, sql)
        {
        }
        public DoQueryIn(string[] inarr, string paramname, string sql) : base(inarr, paramname, sql)
        {
        }
    }
}
