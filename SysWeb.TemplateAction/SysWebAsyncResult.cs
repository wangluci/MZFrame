using System;
using System.Threading;
using TemplateAction.Label;
using TemplateAction.Core;
using System.Web;

namespace SysWeb.TemplateAction
{
    public class SysWebAsyncResult : IAsyncResult, ITAAsyncResult
    {
        internal SysWebAsyncResult(AsyncCallback callBack, HttpContext context)
        {
            _complete = false;
            _asyncCallback = callBack;
            _context = context;
        }
        private bool _complete;
        private HttpContext _context;
        private AsyncCallback _asyncCallback;
        public object AsyncState
        {
            get { return _context; }
        }
        public bool CompletedSynchronously
        {
            get { return false; }
        }
        public bool IsCompleted
        {
            get
            {
                return _complete;
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
            _complete = true;
            if (data != null)
            {
                data.Output();
            }
            _context.ApplicationInstance.CompleteRequest();
            _asyncCallback?.Invoke(this);
        }
    }
}
