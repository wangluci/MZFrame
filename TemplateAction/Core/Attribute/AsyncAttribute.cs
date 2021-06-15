using System;

namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AsyncAttribute : Attribute
    {
        /// <summary>
        /// 超时时间，单位秒
        /// </summary>
        private int _asyncTimeout;
        public int AsyncTimeout {
            get { return _asyncTimeout; }
        }
        public AsyncAttribute(int timeout = 45)
        {
            _asyncTimeout = timeout;
        }
    }
}
