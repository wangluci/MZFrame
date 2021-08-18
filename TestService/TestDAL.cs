using Common.MySql;
using TestService.Model;
using MyAccess.DB;
using System;
using MyAccess.Aop;

namespace TestService
{
    /// <summary>
    /// 同步数据库操作层
    /// </summary>
    public class TestDAL : MySqlSupport
    {
        public TestDAL(string testconnstr) : base(testconnstr) { }
        [Trans]
        public virtual int AddTestRow(testtb tb)
        {
            DoInsert<testtb> di = new DoInsert<testtb>(tb);
            help.DoCommand(di);
            return di.RowCount;
        }
    }
}
