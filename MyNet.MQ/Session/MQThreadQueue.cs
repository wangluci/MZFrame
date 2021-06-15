using MyNet.Common;
using MyNet.Common.TimeWheel;
using System;

namespace MyNet.MQ.Session
{
    public class MQThreadQueue
    {
        private HashedWheelTimer _scheduler;
        public MQThreadQueue()
        {
            _scheduler = new HashedWheelTimer(TimeSpan.FromMilliseconds(400), 100000, 0);
        }

   
        public IWheelTimeout EnqueueMsg(MQMessage msg)
        {
            DateTime overtimes = Converter.Cast<DateTime>(msg.TriggeredDate);
            return _scheduler.NewTimeout(msg, overtimes - DateTime.Now);
        }
        public void Close()
        {
            _scheduler.Stop();
        }
    }
}
