using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TestService
{
    public class Test: TABaseController
    {
        ITestListener _listener;
        public Test(ITestListener listener)
        {
            _listener = listener;
        }
        public ViewResult Index()
        {
            _listener.onTest();
            SetGlobal("a", 1);
            SetGlobal("Hx", "<a href='xxx'>你好啊</a>");
            return View();
        }
    }
}
