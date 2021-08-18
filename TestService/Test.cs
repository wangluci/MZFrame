using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;
using TestService.Model;

namespace TestService
{
    public class Test : TABaseController
    {
        ITestListener _listener;
        TestService _service;
        public Test(ITestListener listener, TestService testservice)
        {
            _listener = listener;
            _service = testservice;
        }
        public ViewResult Index()
        {
            _listener.onTest();
            SetGlobal("a", 1);
            SetGlobal("Hx", "<a href='xxx'>你好啊</a>");
            return View();
        }
        public async Task<TextResult> List()
        {
            List<testtb> tlist = await _service.GetTestListAsync();
            string rt = string.Empty;
            foreach(testtb tb in tlist)
            {
                rt += "id:" + tb.testid + " des:" + tb.testdes + "\n";
            }
            return Content(rt);
        }
        public TextResult TestAdd()
        {
            return Content(_service.TestDAL().ToString());
        }
        public async Task<TextResult> TestAddAsync()
        {
            return Content((await _service.TestDALAsync()).ToString());
        }
        public async Task<TextResult> TestAsync()
        {
            string str1 = string.Format("TestAsync测试异步:ThreadId{0}", Thread.CurrentThread.ManagedThreadId.ToString());
            System.Diagnostics.Debug.WriteLine(str1);
            string rt = await _service.TestTask();
            return Content(rt);
        }
    }
}
