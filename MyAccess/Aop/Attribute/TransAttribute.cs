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

        public TransAttribute(Isolation level = Isolation.DEFAULT)
        {
            _isolation = level;
        }
        public override Task ProceedBefore(IDbHelp dbhelp, IInvocation invocation)
        {
            bool issync = invocation.Method.ReturnType == typeof(void) || !typeof(Task).IsAssignableFrom(invocation.Method.ReturnType);
            if (dbhelp != null)
            {
                if (issync)
                {
                    DBTransScope.Instance().OpenDB(dbhelp, _isolation);
                    return Task.CompletedTask;
                }
                else
                {
                    return DBTransScope.Instance().OpenDBAsync(dbhelp, _isolation);
                }
            }
            else
            {
                DBTransScope.Instance().BeginScope();
                return Task.CompletedTask;
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
                DBTransScope.Instance().RollBack();
                return Task.CompletedTask;
            }
        }
        public override Task ProceedAfter(IDbHelp dbhelp, IInvocation invocation)
        {
            if (dbhelp != null)
            {
                if (DBTransScope.Instance().IsBegan())
                {
                    return Task.CompletedTask;
                }
                if (dbhelp.DbTrans == null)
                {
                    return Task.CompletedTask;
                }
                dbhelp.Commit();
            }
            else
            {
                if (!DBTransScope.Instance().IsBegan())
                {
                    return Task.CompletedTask;
                }
                DBTransScope.Instance().Commit();
            }
            return Task.CompletedTask;
        }


    }
}
