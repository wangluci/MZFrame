using System;
using TemplateAction.Cache;

namespace SysWeb.TemplateAction
{
    public class AsyncTimeoutPool
    {
        static AsyncTimeoutPool() { }
        private HashedWheelTimer _timer;
        private AsyncTimeoutPool()
        {
            _timer = new HashedWheelTimer(TimeSpan.FromMilliseconds(400), 100000, 0);
        }
        private static readonly AsyncTimeoutPool _instance = new AsyncTimeoutPool();
        public static AsyncTimeoutPool Instance
        {
            get
            {
                return _instance;
            }
        }

        public IWheelTimeout AddAsyncResult(SysWebAsyncResult rs, DateTime end)
        {
            return _timer.NewTimeout(rs, end - DateTime.Now);
        }
    }
}
