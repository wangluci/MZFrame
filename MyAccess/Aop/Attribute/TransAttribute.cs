using Castle.DynamicProxy;
using System;
using MyAccess.DB;
using System.Threading.Tasks;

namespace MyAccess.Aop
{

    [AttributeUsage(AttributeTargets.Method)]
    public class TransAttribute : AbstractAopAttr
    {
        private Isolation _isolation;
        public Isolation IsolatioinLevel
        {
            get { return _isolation; }
        }
        public TransAttribute()
        {
            _isolation = Isolation.DEFAULT;
        }
        public TransAttribute(Isolation level)
        {
            _isolation = level;
        }
        public override async Task ProceedBefore(object state, IInvocation invocation)
        {
            bool issync = invocation.Method.ReturnType == typeof(void) || !typeof(Task).IsAssignableFrom(invocation.Method.ReturnType);
            if (state != null)
            {
                IDbHelp dbHelp = (IDbHelp)state;
                if (issync)
                {
                    DBTransMan.Instance().OpenDB(dbHelp, _isolation);
                }
                else
                {
                    await DBTransMan.Instance().OpenDBAsync(dbHelp, _isolation);
                }
            }
            else
            {
                DBTransMan.Instance().BeginTrans();
            }
        }
        public override Task ProceedAfter(object state, Exception ex, IInvocation invocation)
        {
            if (state != null)
            {
                if (DBTransMan.Instance().IsOpenTrans())
                {
                    return Task.CompletedTask;
                }
                IDbHelp dbHelp = (IDbHelp)state;
                if (ex != null)
                {
                    dbHelp.RollBack();
                    return Task.CompletedTask;
                }
                Task rt = invocation.ReturnValue as Task;
                if (rt != null)
                {
                    ITransReturn irt = rt.GetType().GetProperty("Result").GetValue(rt, null) as ITransReturn;
                    if (irt != null)
                    {
                        if (!irt.IsSuccess())
                        {
                            dbHelp.RollBack();
                            return Task.CompletedTask;
                        }

                    }
                    dbHelp.Commit();
                }
                else
                {
                    ITransReturn tr = invocation.ReturnValue as ITransReturn;
                    if (tr != null)
                    {
                        if (!tr.IsSuccess())
                        {
                            dbHelp.RollBack();
                            return Task.CompletedTask;
                        }
                    }
                    dbHelp.Commit();
                }
            }
            else
            {
                if (ex != null)
                {
                    DBTransMan.Instance().RollBack();
                    return Task.CompletedTask;
                }
                Task rt = invocation.ReturnValue as Task;
                if (rt != null)
                {
                    ITransReturn irt = rt.GetType().GetProperty("Result").GetValue(rt, null) as ITransReturn;
                    if (irt != null)
                    {
                        if (!irt.IsSuccess())
                        {
                            DBTransMan.Instance().RollBack();
                            return Task.CompletedTask;
                        }

                    }
                    DBTransMan.Instance().Commit();
                }
                else
                {
                    ITransReturn tr = invocation.ReturnValue as ITransReturn;
                    if (tr != null)
                    {
                        if (!tr.IsSuccess())
                        {
                            DBTransMan.Instance().RollBack();
                            return Task.CompletedTask;
                        }
                    }
                    DBTransMan.Instance().Commit();
                }

            }
            return Task.CompletedTask;
        }


    }
}
