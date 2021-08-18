using Common.MySql;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestService.Model;

namespace TestService
{
    /// <summary>
    /// 异步数据库操作层
    /// </summary>
    public class TestDALAsync: MySqlSupport
    {
        public TestDALAsync(string testconnstr) : base(testconnstr) { }
        public virtual async Task<List<testtb>> GetTesttbsAsync()
        {
            DoQuerySql<testtb> dqs = new DoQuerySql<testtb>("select * from testtb");
            await help.DoCommandAsync(dqs);
            return dqs.ToList();
        }
        public virtual async Task<int> AddTestRow(testtb tb)
        {
            DoInsert<testtb> di = new DoInsert<testtb>(tb);
            await help.DoCommandAsync(di);
            return di.RowCount;
        }
    }
}
