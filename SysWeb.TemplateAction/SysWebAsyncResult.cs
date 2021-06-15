using System;
using System.Threading;
using TemplateAction.Label;
using TemplateAction.Core;
using TemplateAction.Cache;
using System.Web;

namespace SysWeb.TemplateAction
{
    public class SysWebAsyncResult : IAsyncResult, ITAAsyncResult,TimerTask
    {
        internal SysWebAsyncResult(AsyncCallback callBack, TARequestHandleBuilder builder)
        {
            _complete = false;
            _asyncCallback = callBack;
            _builder = builder;
            if (builder.Async.AsyncTimeout > 0)
            {
                //设定超时
                _wheel = AsyncTimeoutPool.Instance.AddAsyncResult(this, DateTime.Now.AddSeconds(builder.Async.AsyncTimeout));
            }
            builder.StartAsync(this);
        }
        private bool _complete;
        private TARequestHandleBuilder _builder;
        private AsyncCallback _asyncCallback;
        private IWheelTimeout _wheel;
        public object AsyncState
        {
            get { return _builder.Context; }
        }
        public bool CompletedSynchronously
        {
            get { return false; }
        }
        public bool IsCompleted
        {
            get {
                if (_wheel == null)
                {
                    return _complete;
                }
                else
                {
                    return _wheel.Expired || _wheel.Cancelled;
                }
            }
        }
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }
        public void Completed(IResult data)
        {
            bool canexe = false;
            if (_wheel == null)
            {
                canexe = true;
                _complete = true;
            }
            else
            {
                canexe = _wheel.Cancel();
            }
            if (canexe)
            {
                if (data != null)
                {
                    data.Output();
                }
                ((SysWebContext)_builder.Context).CompleteRequest();
                _asyncCallback?.Invoke(this);
            }
        }
        public void Timeout()
        {
            _builder.Context.Response.StatusCode = 503;
            ((SysWebContext)_builder.Context).CompleteRequest();
            _asyncCallback?.Invoke(this);
        }

        public void Run(IWheelTimeout timeout)
        {
            Timeout();
        }
    }
}
