using Castle.DynamicProxy;
using System;
using MyAccess.Aop.DAL;
using MyAccess.DB;

namespace MyAccess.Aop
{

    [AttributeUsage(AttributeTargets.Method)]
    public class TransAttribute : DBAbstractAttr
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
        public override bool InterceptDeal(IDBSupport support, IInvocation invocation)
        {
            //事务已开启，则不执行事务
            if (support.IsTranslation)
            {
                invocation.Proceed();
            }
            else
            {
                try
                {
                    support.DBHelp.BeginTran(_isolation);
                    invocation.Proceed();
                    ITransReturn tr = invocation.ReturnValue as ITransReturn;
                    if (tr != null)
                    {
                        if (tr.IsSuccess())
                        {
                            support.DBHelp.Commit();
                        }
                    }
                    else
                    {
                        support.DBHelp.Commit();
                    }
                }
                finally
                {
                    support.DBHelp.RollBack();
                }
            }

            return true;
        }

    }
}
