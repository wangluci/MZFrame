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
        public TestDALAsync() : base("server=127.0.0.1;user id=root;password=测试密码;database=test;") { }
    }
}
