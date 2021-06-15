using System;
namespace MyNet.Handlers
{
    /// <summary>
    /// 定时触发指定事件
    /// </summary>
    public class LoopTimeoutHandler : AbstractChannelHandler
    {
        protected string _evtname;
        protected int _repeatInterval;
        protected bool _canloop;

        protected void LoopEx(IContext context)
        {
            if (!context.Channel.Active)
            {
                return;
            }
            if (!_canloop) return;
            context.Channel.Emit(_evtname, EventArgs.Empty);
            context.Loop.Schedule((c) =>
            {
                LoopEx(c);
            }, context, _repeatInterval);
        }
        public LoopTimeoutHandler(int milliseconds, string evtname)
        {
            _repeatInterval = milliseconds;
            _evtname = evtname;
        }
        public override void HandlerInstalled(IContext context)
        {
            _canloop = true;
            context.Loop.Schedule((c) =>
            {
                LoopEx(c);
            }, context, _repeatInterval);
        }

        public override void HandlerUninstalled(IContext context)
        {
            _canloop = false;
        }
    }
}
