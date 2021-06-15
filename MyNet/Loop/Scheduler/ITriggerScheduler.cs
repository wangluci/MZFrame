using System;

namespace MyNet.Loop.Scheduler
{
    public interface ITriggerScheduler
    {
        void RemoveTrigger(ITriggerRunnable task);
        /// <summary>
        /// 定时触发
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        ITriggerRunnable Schedule(ITriggerRunnable task);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval">单位毫秒</param>
        /// <returns></returns>
        ITriggerRunnable Schedule(Action action, int interval);
        ITriggerRunnable Schedule<T>(Action<T> action, T state, int interval);
        ITriggerRunnable Schedule<E, T>(Action<E, T> action, E context, T state, int interval);
    }
}
