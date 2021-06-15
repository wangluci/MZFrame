using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class SimpleRunnable : IRunnable
    {
        private Action _ac;
        public SimpleRunnable(Action ac)
        {
            _ac = ac;
        }

        public void Run()
        {
            _ac();
        }
    }
}
