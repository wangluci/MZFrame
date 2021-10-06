using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class MvcMiddleware : IFilterMiddleware
    {
        /// <summary>
        /// 参数绑定
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pi"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool Mapping(TAAction ac, ParameterInfo pi, out object result)
        {
            IEnumerable<AbstractMappingAttribute> attrs = pi.GetCustomAttributes<AbstractMappingAttribute>();
            int attrc = 0;
            foreach (AbstractMappingAttribute att in attrs)
            {
                attrc++;
                if (att.Mapping(ac, pi.Name, pi.ParameterType, out result))
                {
                    return true;
                }
            }
            if (attrc == 0)
            {
                LinkedListNode<IParamMapping> node = ac.Context.Application.FirstParamMapping();
                if (node.Value.Mapping(node.Next, ac, pi.Name, pi.ParameterType, out result))
                {
                    return true;
                }
            }
            if (pi.DefaultValue != DBNull.Value)
            {
                result = pi.DefaultValue;
                return true;
            }
            result = null;
            return false;
        }
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
                    for (int i = 0; i < pinfos.Length; i++)
                    {
                        //创建参数映射
                        object mapobj;
                        if (Mapping(ac, pinfos[i], out mapobj))
                        {
                            paramlist.Add(mapobj);
                        }
                        else
                        {
                            return null;
                        }
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
