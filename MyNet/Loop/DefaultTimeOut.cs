using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class DefaultTimeOut<T> : TriggerRunnable
    {
        private Action<T> _ac;
        private T _state;
        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="state"></param>
        /// <param name="interval">单位毫秒</param>
        public DefaultTimeOut(ITriggerScheduler executor, Action<T> ac, T state, int interval) : base(executor, interval)
        {
            _ac = ac;
            _state = state;
        }

        protected override void Execute()
        {
            _ac(_state);
        }
    }
}
