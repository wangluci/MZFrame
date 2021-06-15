using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class DefaultRunnable<T> : IRunnable
    {
        private Action<T> _ac;
        private T _state;
        public DefaultRunnable(Action<T> ac, T state)
        {
            _ac = ac;
            _state = state;
        }

        public void Run()
        {
            _ac(_state);
        }
    }
}
