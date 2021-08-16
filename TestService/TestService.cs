
using System;
using System.Threading;
using System.Threading.Tasks;
using TestService.Model;
namespace TestService
{
    public class TestService
    {
        private TestDAL _dal;
        private TestDALAsync _dalAsync;
        public TestService(TestDAL dal, TestDALAsync dalAsync)
        {
            _dal = dal;
            _dalAsync = dalAsync;
        }
        public virtual int TestDAL()
        {
            testtb tb = MyAccess.Aop.InterceptFactory.CreateEntityOp<testtb>();
            tb.testdes = "测试同步添加";
            return _dal.AddTestRow(tb);
        }
        public virtual async Task<int> TestDALAsync()
        {
            testtb tb = MyAccess.Aop.InterceptFactory.CreateEntityOp<testtb>();
            tb.testdes = "测试同步添加";
            return await _dalAsync.AddTestRow(tb);
        }
        public virtual async Task<string> TestTask()
        {
            string str1 = string.Format("TestTask测试异步:ThreadId{0}", Thread.CurrentThread.ManagedThreadId.ToString());
            System.Diagnostics.Debug.WriteLine(str1);
            string rt = await Task.Run(() =>
              {
                  string str = string.Format("TestTaskRun测试异步:ThreadId{0}", Thread.CurrentThread.ManagedThreadId.ToString());
                  System.Diagnostics.Debug.WriteLine(str);
                  Thread.Sleep(1000);
                  return str;
              });
            string str2 = string.Format("TestTaskAfter测试异步:ThreadId{0}", Thread.CurrentThread.ManagedThreadId.ToString());
            System.Diagnostics.Debug.WriteLine(str2);
            return rt;
        }
    }
}
