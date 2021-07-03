using System;
using System.Collections.Generic;
using System.Text;

namespace TestService
{
    public interface ITestListener
    {
        /// <summary>
        /// 测试事件
        /// </summary>
        void onTest();
    }
}
