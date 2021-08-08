﻿using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyAccess.Aop
{
    /// <summary>
    /// 业务层拦截器
    /// </summary>
    public class BLLIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TransAttribute dbtrans = (TransAttribute)invocation.MethodInvocationTarget.GetCustomAttribute(typeof(TransAttribute));
            if (dbtrans != null)
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
            else
            {
                invocation.Proceed();
            }

        }
    }
}
