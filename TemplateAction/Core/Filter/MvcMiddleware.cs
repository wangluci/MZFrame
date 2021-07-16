using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class MvcMiddleware : IFilterMiddleware
    {
        protected const string CONTROLLER_PRE = "CONTR_STORE_";
        public object Excute(TARequestHandle request, FilterMiddlewareNode next)
        {
            if (request.ControllerNode == null) return null;
            if (request.ActionNode == null) return null;
            ITAContext context = request.Context;
            Type ct = request.ControllerNode.ControllerType;
            //创建控制器
            string tmpcontrollkey = CONTROLLER_PRE + ct.FullName;
            IController c = context.Items[tmpcontrollkey] as IController;
            if (c == null)
            {
                c = context.Application.CreateInstance(ct) as IController;
                context.Items[tmpcontrollkey] = c;
            }
            if (c == null)
            {
                return null;
            }
            try
            {
                //绑定控制器
                c.Init(request);
                if (!request.ActionNode.JudgeHttpMethod(context.Request.HttpMethod)) return null;
                MethodInfo method = request.ActionNode.Method;
                ParameterInfo[] pinfos = method.GetParameters();
                List<object> paramlist = new List<object>();
                //开始遍历参数进行验证
                ITAObjectCollection gc = null;
                if (context.Request.Form == null)
                {
                    gc = context.Request.Query;
                }
                else
                {
                    gc = new TAGroupCollection(context.Request.Query, context.Request.Form);
                }
                if (!Equals(request.ExtParams, null))
                {
                    gc = new TAGroupCollection(gc, request.ExtParams);
                }

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
