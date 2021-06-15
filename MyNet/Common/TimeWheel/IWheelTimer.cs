﻿using System;
using System.Collections.Generic;


namespace MyNet.Common.TimeWheel
{
    public interface IWheelTimer
    {
        /// <summary>
        ///  Schedules the specified TimerTask for one-time execution after the specified delay.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="span"></param>
        /// <returns> handle which is associated with the specified task</returns>
        IWheelTimeout NewTimeout(TimerTask task, TimeSpan span);


        /// <summary>
        /// Releases all resources acquired by this Timer and cancels all
        /// tasks which were scheduled but not executed yet.
        /// </summary>
        /// <returns>the handles associated with the tasks which were canceled by this method</returns>
        ISet<IWheelTimeout> Stop();
    }
}
