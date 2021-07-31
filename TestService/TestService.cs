
using System.Threading;
using System.Threading.Tasks;

namespace TestService
{
    public class TestService
    {
        public async Task<string> TestTask()
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
