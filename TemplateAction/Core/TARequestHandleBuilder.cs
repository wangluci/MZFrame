using System;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TARequestHandleBuilder
    {
        private ITAContext _context;
        public ITAContext Context
        {
            get { return _context; }
        }

        private AsyncAttribute _async;
        public AsyncAttribute Async
        {
            get { return _async; }
        }
        private TARequestHandle _request;
        public TARequestHandleBuilder(ITAContext context, ControllerNode controller, ActionNode action, ITAObjectCollection exparams)
        {
            _async = null;
            _context = context;
            _request = new TARequestHandle(context, controller, action, exparams);

            if (_request.ActionNode == null) return;
            if (_request.ActionNode.Async == null) return;
            _async = _request.ActionNode.Async;
        }

        /// <summary>
        /// .net framework 异步执行
        /// </summary>
        /// <param name="res"></param>
        public void StartAsync(ITAAsyncResult res)
        {
            if (_request.ActionNode.Method.ReturnType.IsAssignableFrom(typeof(Task)))
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
            else
            {
                Task.Run(() =>
                {
                    res.Completed(this.BuildAndExcute());
                });
            }
        }
        /// <summary>
        /// .net core 异步执行
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            if (_request.ActionNode.Method.ReturnType.IsAssignableFrom(typeof(Task)))
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
            else
            {
                return Task.Run(this.BuildAndExcute);
            }
        }

        /// <summary>
        /// 同步执行
        /// </summary>
        /// <returns></returns>
        public IResult BuildAndExcute()
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
