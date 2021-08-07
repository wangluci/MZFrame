using Common.MySql;
using MyAccess.Aop.DAL;
using System;
using TemplateAction.Core;

namespace TestService
{
    /// <summary>
    /// 同步数据库操作层
    /// </summary>
    public class TestDAL : MySqlSupport
    {
        public TestDAL() : base("server=127.0.0.1;user id=root;password=测试密码;database=test;") { }
    }
}
