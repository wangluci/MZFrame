using Common.MySql;
using MyAccess.Aop.DAL;
using System;

namespace TestService
{
    /// <summary>
    /// 异步数据库操作层
    /// </summary>
    public class TestDALAsync: MySqlSupportAsync
    {
        public TestDALAsync(IDBConfig config) : base(config) { }
    }
}
