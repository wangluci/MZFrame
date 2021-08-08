using Castle.DynamicProxy;
using System;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System.Runtime.CompilerServices;
using System.Reflection;

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
        public override bool InterceptDeal(IInvocation invocation)
        {
            IDBSupport support = invocation.InvocationTarget as IDBSupport;
            //判断是否为DAL层的拦截器
            if (support != null)
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

            
            }
            else
            {
                //判断方法是否为异步
                Attribute attrib = invocation.Method.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
                if (attrib != null)
                {
                    //异步
                    try
                    {
                        DBManAsync.Instance().BeginTrans();
                        invocation.Proceed();
                        ITransReturn tr = invocation.ReturnValue as ITransReturn;
                        if (tr != null)
                        {
                            if (tr.IsSuccess())
                            {
                                DBManAsync.Instance().Commit();
                            }
                        }
                        else
                        {
                            DBManAsync.Instance().Commit();
                        }
                    }
                    finally
                    {
                        DBManAsync.Instance().RollBack();
                    }
                }
                else
                {
                    //同步
                    try
                    {
                        DBMan.Instance().BeginTrans();
                        invocation.Proceed();
                        ITransReturn tr = invocation.ReturnValue as ITransReturn;
                        if (tr != null)
                        {
                            if (tr.IsSuccess())
                            {
                                DBMan.Instance().Commit();
                            }
                        }
                        else
                        {
                            DBMan.Instance().Commit();
                        }
                    }
                    finally
                    {
                        DBMan.Instance().RollBack();
                    }
                }
            }
            return true;
        }

    }
}
