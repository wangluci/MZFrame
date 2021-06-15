using MyNet.Handlers;
using MyNet.Loop.Scheduler;
using System.Threading;
using TemplateAction.Label;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MyNetAsyncResult : ITAAsyncResult
    {
        private TARequestHandleBuilder _builder;
        private ITriggerRunnable _runnable;
        public MyNetAsyncResult(TARequestHandleBuilder builder)
        {
            _builder = builder;
        }
        public void SetRunnable(ITriggerRunnable runnable)
        {
            _runnable = runnable;
        }
        public void Completed(IResult data)
        {
            bool cansuccess = true;
            if (_runnable != null)
            {
                cansuccess = _runnable.Cancel();
            }
            if (cansuccess)
            {
                if (data != null)
                {
                    data.Output();
                }
                ((HttpContext)_builder.Context).RequestFinish();
            }
        }

        public void Timeout()
        {
            _builder.Context.Response.StatusCode = 503;
            ((HttpContext)_builder.Context).RequestFinish();
        }
    }
}
