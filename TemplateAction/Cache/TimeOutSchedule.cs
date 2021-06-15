using System;
namespace TemplateAction.Cache
{
    public class TimeOutSchedule : TimerTask
    {
        /// <summary>
        /// 缓存池
        /// </summary>
        public CachePool Pool { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string ItemKey { get; set; }
        public void Run(IWheelTimeout timeout)
        {
            try
            {
                Pool.Remove(ItemKey);
            }
            catch (Exception) { }
        }
    }
}
