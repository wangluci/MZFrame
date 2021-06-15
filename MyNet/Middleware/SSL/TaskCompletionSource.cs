﻿using System;
using System.Threading.Tasks;

namespace MyNet.Middleware.SSL
{
    public class TaskCompletionSource : TaskCompletionSource<int>
    {
        public static readonly TaskCompletionSource Void = CreateVoidTcs();

        public TaskCompletionSource(object state)
            : base(state)
        {
        }

        public TaskCompletionSource()
        {
        }

        public bool TryComplete()
        {
            return this.TrySetResult(0);
        }

        public void Complete()
        {
            this.SetResult(0);
        }

        // todo: support cancellation token where used
        public bool SetUncancellable()
        {
            return true;
        }

        public override string ToString()
        {
            return "TaskCompletionSource[status: " + this.Task.Status.ToString() + "]";
        }

        static TaskCompletionSource CreateVoidTcs()
        {
            var tcs = new TaskCompletionSource();
            tcs.TryComplete();
            return tcs;
        }
    }
}
