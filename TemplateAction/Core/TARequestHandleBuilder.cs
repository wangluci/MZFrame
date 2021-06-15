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
        private string _ns;
        private string _controller;
        private string _action;
        private ITAObjectCollection _exparams;
        private Type _controllerType;
        private ActionNode _node;
        private AsyncAttribute _async;
        public AsyncAttribute Async
        {
            get { return _async; }
        }

        public TARequestHandleBuilder(ITAContext context, string ns, string controller, string action, ITAObjectCollection exparams, Type controllerType, ActionNode node)
        {
            _context = context;
            _ns = ns;
            _controller = controller;
            _action = action;
            _exparams = exparams;
            _controllerType = controllerType;
            _node = node;
        }

        /// <summary>
        /// 开始执行异步
        /// </summary>
        /// <param name="res"></param>
        public void StartAsync(ITAAsyncResult res)
        {
            if (_node.Method.ReturnType.IsAssignableFrom(typeof(Task)))
            {
                TARequestHandle rh = new TARequestHandle(_context, _ns, _controller, _action, _exparams);
                Task rt = rh.Excute(_controllerType, _node) as Task;
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

        public Task StartAsync()
        {
            if (_node.Method.ReturnType.IsAssignableFrom(typeof(Task)))
            {
                TARequestHandle rh = new TARequestHandle(_context, _ns, _controller, _action, _exparams);
                Task rt = rh.Excute(_controllerType, _node) as Task;
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
        public bool CreateAsync()
        {
            if (_node == null) return false;
            if (_node.Async == null) return false;
            _async = _node.Async;
            return true;
        }

        public IResult BuildAndExcute()
        {
            TARequestHandle rh = new TARequestHandle(_context, _ns, _controller, _action, _exparams);
            IResult rt = rh.Excute(_controllerType, _node) as IResult;
            if (rt == null)
            {
                return new V404Result(_context);
            }
            return rt;
        }

    }
}
