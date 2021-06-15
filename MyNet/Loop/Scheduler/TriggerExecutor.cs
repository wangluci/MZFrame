using MyNet.Common;
using MyNet.Loop;
using System;
namespace MyNet.Loop.Scheduler
{
    /// <summary>
    /// 执行触发任务
    /// </summary>
    public abstract class TriggerExecutor : IExecutor, ITriggerScheduler
    {
        private BinaryHeap<ITriggerRunnable> mTriggers = new BinaryHeap<ITriggerRunnable>();

        /// <summary>
        /// 弹出下一个到期触发器
        /// </summary>
        /// <returns></returns>
        protected ITriggerRunnable PopNextTrigger()
        {
            ITriggerRunnable tr = mTriggers.Peek();
            if (tr == null) return null;
            long nowticks = Converter.Cast<long>(DateTime.Now);
            if (tr.DeadTime <= nowticks)
            {
                return mTriggers.Dequeue();
            }
            else
            {
                return null;
            }
        }

        protected ITriggerRunnable PeekTrigger()
        {
            return mTriggers.Peek();
        }

        public void Execute<T>(Action<T> action, T state)
        {
            Execute(new DefaultRunnable<T>(action, state));
        }
        public void Execute<E, T>(Action<E, T> action, E context, T state)
        {
            Execute(new TwoRunnable<E, T>(action, context, state));
        }
        public void RemoveTrigger(ITriggerRunnable task)
        {
            if (InCurrentThread())
            {
                mTriggers.Remove(task);
            }
            else
            {
                this.Execute((e, t) =>
                {
                    e.RemoveTrigger(t);
                }, this, task);
            }
        }
        public ITriggerRunnable Schedule(ITriggerRunnable task)
        {
            if (InCurrentThread())
            {
                mTriggers.Enqueue(task);
            }
            else
            {
                this.Execute((e, t) =>
                {
                    e.mTriggers.Enqueue(t);
                }, this, task);
            }
            return task;
        }

        public ITriggerRunnable Schedule(Action action, int interval)
        {
            return Schedule(new SimpleTimeOut(this, action, interval));
        }
        public ITriggerRunnable Schedule<T>(Action<T> action, T state, int interval)
        {
            return Schedule(new DefaultTimeOut<T>(this, action, state, interval));
        }

        public ITriggerRunnable Schedule<E, T>(Action<E, T> action, E context, T state, int interval)
        {
            return Schedule(new TwoTimeOut<E, T>(this, action, context, state, interval));
        }

        public abstract bool InCurrentThread();
        public abstract void Execute(IRunnable task);
        public abstract void Close();

    }
}
