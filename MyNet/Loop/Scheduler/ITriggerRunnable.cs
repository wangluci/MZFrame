﻿using MyNet.Common;
using System;

namespace MyNet.Loop.Scheduler
{
    /// <summary>
    /// 定时触发任务
    /// </summary>
    public interface ITriggerRunnable : IRunnable, IComparable
    {
        /// <summary>
        /// 触发间隔
        /// </summary>
        int RepeatInterval { get; }
        /// <summary>
        /// 下次触发时间
        /// </summary>
        long DeadTime { get; set; }
        bool Cancel();
    }
}
