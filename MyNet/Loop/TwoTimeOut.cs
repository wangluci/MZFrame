using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class TwoTimeOut<T1,T2> : TriggerRunnable
    {
        private Action<T1, T2> _ac;
        private T1 _state1;
        private T2 _state2;
        public TwoTimeOut(ITriggerScheduler executor, Action<T1, T2> ac, T1 state1, T2 state2, int interval) : base(executor, interval)
        {
            _ac = ac;
            _state1 = state1;
            _state2 = state2;
        }

        protected override void Execute()
        {
            _ac(_state1, _state2);
        }
    }
}
