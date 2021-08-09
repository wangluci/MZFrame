using Common.MySql;

namespace TestService
{
    /// <summary>
    /// 同步数据库操作层
    /// </summary>
    public class TestDAL : MySqlSupport
    {
        public TestDAL(string testconnstr) : base(testconnstr) { }
    }
}
