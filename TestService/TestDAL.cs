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
        public TestDAL(IDBConfig config) : base(config) { }
    }
}
