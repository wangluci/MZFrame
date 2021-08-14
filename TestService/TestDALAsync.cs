﻿using Common.MySql;
using MyAccess.DB;
using System;
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
        public virtual async Task<int> AddTestRow(testtb tb)
        {
            DoInsert di = new DoInsert(tb);
            await help.DoCommandAsync(di);
            Thread.Sleep(5000);
            return di.RowCount;
        }
    }
}
