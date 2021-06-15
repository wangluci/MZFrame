using MyNet.Common;
using MyNet.Loop.Scheduler;
using System;
using System.Threading;

namespace MyNet.Loop
{
    public abstract class TriggerRunnable : ITriggerRunnable
    {
        internal const int TRIGGER_INIT = 0;
        internal const int TRIGGER_CANCELLED = 1;
        internal const int TRIGGER_EXPIRED = 2;
        /// <summary>
        /// 0为初始状态，1为取消，2为过期
        /// </summary>
        protected volatile int _runstate = TRIGGER_INIT;
        protected long _nextTime;
        protected int _repeatInterval;
        protected ITriggerScheduler _executor;
        /// <summary>
        /// 默认值设置
        /// </summary>
        /// <param name="interval">单位毫秒</param>
        /// <param name="repeat"></param>
        public TriggerRunnable(ITriggerScheduler executor, int interval)
        {
            _executor = executor;
            _nextTime = Converter.Cast<long>(DateTime.Now) + interval;
            _repeatInterval = interval;
        }
        protected bool CompareAndSetState(int expected, int state)
        {
            int originalState = Interlocked.CompareExchange(ref _runstate, state, expected);
            return originalState == expected;
        }

        public bool Cancel()
        {
            if (!CompareAndSetState(TRIGGER_INIT, TRIGGER_CANCELLED))
            {
                return false;
            }
            _executor.RemoveTrigger(this);
            return true;
        }
        public long DeadTime
        {
            get
            {
                return _nextTime;
            }

            set
            {
                _nextTime = value;
            }
        }


        public int RepeatInterval
        {
            get
            {
                return _repeatInterval;
            }
        }

        public void Run()
        {
            if (!CompareAndSetState(TRIGGER_INIT, TRIGGER_EXPIRED))
            {
                return;
            }
            Execute();
        }
        protected abstract void Execute();
        public int CompareTo(object obj)
        {
            ITriggerRunnable cmp = obj as ITriggerRunnable;
            if (cmp == null)
            {
                return -1;
            }
            return this._nextTime.CompareTo(cmp.DeadTime);
        }
    }
}
