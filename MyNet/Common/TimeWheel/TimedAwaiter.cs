﻿using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace MyNet.Common.TimeWheel
{
    public sealed class TimedAwaiter : INotifyCompletion, TimerTask
    {
        public bool IsCompleted { get; private set; }


        private Action _continuation;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            if (IsCompleted)
                Interlocked.Exchange(ref _continuation, null)?.Invoke();
        }

        public void Run(IWheelTimeout timeout)
        {
            IsCompleted = true;
            Interlocked.Exchange(ref _continuation, null)?.Invoke();
        }

        public TimedAwaiter GetAwaiter()
        {
            return this;
        }

        public object GetResult()
        {
            return null;
        }
    }
}
