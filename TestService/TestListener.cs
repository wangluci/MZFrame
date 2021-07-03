using System;
using System.Collections.Generic;
using System.Text;

namespace TestService
{
    public class TestListener : ITestListener
    {
        public void onTest()
        {
            System.Diagnostics.Debug.WriteLine("测试监听");
        }
    }
}
