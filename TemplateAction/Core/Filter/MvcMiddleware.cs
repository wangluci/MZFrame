using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class MvcMiddleware : IFilterMiddleware
    {
        public object Excute(TAAction ac, FilterMiddlewareNode next)
        {
            if (ac.ControllerNode == null) return null;
            if (ac.ActionNode == null) return null;
            ITAContext context = ac.Context;
            Type ct = ac.ControllerNode.ControllerType;
            //创建控制器
            IController c = context.Application.ServiceProvider.CreateScopeService(ac, ct) as IController;
            if (c == null)
            {
                return null;
            }
            //初始化请求的异常处理
            ac.ExceptionFun = c.Exception;
            try
            {
                //绑定控制器
                c.Init(ac);
                if (!ac.ActionNode.JudgeHttpMethod(context.Request.HttpMethod)) return null;
                MethodInfo method = ac.ActionNode.Method;
                ParameterInfo[] pinfos = method.GetParameters();

                //开始遍历参数进行验证
                if (pinfos.Length > 0)
                {
                    List<object> paramlist = new List<object>();
                    ITAObjectCollection gc = new TAGroupCollection(ac);
                    for (int i = 0; i < pinfos.Length; i++)
                    {
                        //创建参数映射
                        object mapobj = MappingFactory.Mapping(gc, pinfos[i]);
                        if (Equals(mapobj, null))
                        {
                            return null;
                        }
                        paramlist.Add(mapobj);
                    }
                    return c.CallAction(method, paramlist.ToArray());
                }
                else
                {
                    return c.CallAction(method, Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return c.Exception(ex.InnerException);
                }
                else
                {
                    return c.Exception(ex);
                }
            }
        }
    }
}
