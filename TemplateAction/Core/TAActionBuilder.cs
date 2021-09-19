using System;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TAActionBuilder
    {
        private ITAContext _context;
        public ITAContext Context
        {
            get { return _context; }
        }

        private bool _async;
        public bool Async
        {
            get { return _async; }
        }
        private TAAction _request;
        public TAActionBuilder(ITAContext context, ControllerNode controller, ActionNode action, ITAObjectCollection exparams)
        {
            _async = false;
            _context = context;
            _request = new TAAction(context, controller, action, exparams);

            if (_request.ActionNode == null) return;
            _async = _request.ActionNode.Async;
        }

        /// <summary>
        /// 回调用异步执行
        /// </summary>
        /// <param name="res"></param>
        public void StartAsync(ITAAsyncResult res)
        {
            Task rt = _request.Excute() as Task;
            if (rt == null)
            {
                res.Completed(new V404Result(_context));
            }
            else
            {
                rt.ContinueWith((t) =>
                {
                    if (t.Exception != null)
                    {
                        Exception ex = t.Exception;
                        if (t.Exception.InnerExceptions.Count > 0)
                        {
                            ex = t.Exception.InnerExceptions[0];
                        }
                        if (_request.ExceptionFun != null)
                        {
                            res.Completed(_request.ExceptionFun.Invoke(ex));
                        }
                        else
                        {
                            res.Completed(new TextResult(_context, ex.Message));
                        }
                        return;
                    }
                  
                    IResult irt = t.GetType().GetProperty("Result").GetValue(t, null) as IResult;
                    if (irt == null)
                    {
                        res.Completed(new V404Result(_context));
                    }
                    else
                    {
                        res.Completed(irt);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }
        /// <summary>
        /// Task异步执行
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            Task rt = _request.Excute() as Task;
            if (rt != null)
            {
                return rt;
            }
            else
            {
                TaskCompletionSource<IResult> taskSource = new TaskCompletionSource<IResult>();
                taskSource.SetResult(new V404Result(_context));
                return taskSource.Task;
            }
        }

        /// <summary>
        /// 同步执行
        /// </summary>
        /// <returns></returns>
        public IResult Start()
        {
            IResult rt = _request.Excute() as IResult;
            if (rt == null)
            {
                return new V404Result(_context);
            }
            return rt;
        }

    }
}
