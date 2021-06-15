using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class SimpleTimeOut : TriggerRunnable
    {
        private Action _ac;
        public SimpleTimeOut(ITriggerScheduler executor, Action ac, int interval) : base(executor, interval)
        {
            _ac = ac;
        }

        protected override void Execute()
        {
            _ac();
        }
    }
}
