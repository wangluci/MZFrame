using Castle.DynamicProxy;
using System;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System.Runtime.CompilerServices;
using System.Reflection;
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
        public override bool InterceptDeal(IInvocation invocation)
        {

            //判断方法是否为异步
            Attribute attrib = invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AsyncStateMachineAttribute));
            if (attrib != null)
            {
                //异步
                DBSupportBase support = invocation.InvocationTarget as DBSupportBase;
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
                            Task rt = invocation.ReturnValue as Task;
                            rt.ContinueWith((t) =>
                            {
                                if (t.Status == TaskStatus.RanToCompletion)
                                {

                                    ITransReturn irt = t.GetType().GetProperty("Result").GetValue(t, null) as ITransReturn;
                                    if (irt == null)
                                    {
                                        support.DBHelp.Commit();
                                    }
                                    else
                                    {
                                        if (irt.IsSuccess())
                                        {
                                            support.DBHelp.Commit();
                                        }
                                        else
                                        {
                                            support.DBHelp.RollBack();
                                        }
                                    }
                                }
                                else
                                {
                                    support.DBHelp.RollBack();
                                }
 
                            }, TaskContinuationOptions.ExecuteSynchronously);
                        }
                        catch
                        {
                            support.DBHelp.RollBack();
                        }
                    }
                }
                else
                {
                    try
                    {
                        DBManAsync.Instance().BeginTrans(_isolation);
                        invocation.Proceed();
                        Task rt = invocation.ReturnValue as Task;
                        rt.ContinueWith((t) =>
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                            {

                                ITransReturn irt = t.GetType().GetProperty("Result").GetValue(t, null) as ITransReturn;
                                if (irt == null)
                                {
                                    DBManAsync.Instance().Commit();
                                }
                                else
                                {
                                    if (irt.IsSuccess())
                                    {
                                        DBManAsync.Instance().Commit();
                                    }
                                    else
                                    {
                                        DBManAsync.Instance().RollBack();
                                    }
                                }
                            }
                            else
                            {
                                DBManAsync.Instance().RollBack();
                            }

                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch
                    {
                        DBManAsync.Instance().RollBack();
                    }

                }
                
            }
            else
            {
                //同步
                DBSupportBase support = invocation.InvocationTarget as DBSupportBase;
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
                    try
                    {
                        DBMan.Instance().BeginTrans(_isolation);
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
