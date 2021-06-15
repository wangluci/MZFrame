using System;

namespace MyNet.Loop.Scheduler
{
    public interface IExecutor
    {
        void Close();
        bool InCurrentThread();
        /// <summary>
        /// 压入队列执行
        /// </summary>
        /// <param name="task"></param>
        void Execute(IRunnable task);
        void Execute<T>(Action<T> action, T state);
        void Execute<E,T>(Action<E, T> action, E context, T state);

    }
}
