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
        public override async Task ProceedBefore(IDbHelp dbhelp, IInvocation invocation)
        {
            bool issync = invocation.Method.ReturnType == typeof(void) || !typeof(Task).IsAssignableFrom(invocation.Method.ReturnType);
            if (dbhelp != null)
            {
                if (issync)
                {
                    DBTransMan.Instance().OpenDB(dbhelp, _isolation);
                }
                else
                {
                    await DBTransMan.Instance().OpenDBAsync(dbhelp, _isolation);
                }
            }
            else
            {
                DBTransMan.Instance().BeginTrans();
            }
        }
        public override Task ProceedException(IDbHelp dbhelp, IInvocation invocation)
        {
            if (dbhelp != null)
            {
                dbhelp.RollBack();
                return Task.CompletedTask;
            }
            else
            {
                DBTransMan.Instance().RollBack();
                return Task.CompletedTask;
            }
        }
        public override Task ProceedAfter(IDbHelp dbhelp, IInvocation invocation)
        {
            if (dbhelp != null)
            {
                if (DBTransMan.Instance().IsOpenTrans())
                {
                    return Task.CompletedTask;
                }
                if (!dbhelp.IsTran())
                {
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
                            dbhelp.RollBack();
                            return Task.CompletedTask;
                        }

                    }
                    dbhelp.Commit();
                }
                else
                {
                    ITransReturn tr = invocation.ReturnValue as ITransReturn;
                    if (tr != null)
                    {
                        if (!tr.IsSuccess())
                        {
                            dbhelp.RollBack();
                            return Task.CompletedTask;
                        }
                    }
                    dbhelp.Commit();
                }
            }
            else
            {
                if (!DBTransMan.Instance().IsOpenTrans())
                {
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
