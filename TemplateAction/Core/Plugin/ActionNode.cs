using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 动作树节点
    /// </summary>
    public class ActionNode : Node
    {
        private MethodInfo mMethod;
        public MethodInfo Method
        {
            get { return mMethod; }
        }

        private bool _async;
        /// <summary>
        /// 判断是否为异步方法
        /// </summary>
        public bool Async
        {
            get { return _async; }
        }
        /// <summary>
        /// 关联模块
        /// </summary>
        private string mAboutModule;
        public string AboutModule
        {
            get { return mAboutModule; }
        }

        /// <summary>
        /// 关联动作
        /// </summary>
        private string mAboutAction;
        public string AboutAction
        {
            get { return mAboutAction; }
        }

        private byte _allowHttpMethod;
        public byte AllowHttpMethod
        {
            get { return _allowHttpMethod; }
        }
        private enum EHttpMethod : byte
        {
            Get = 1,
            Post = 2,
            Delete = 4,
            Put = 8
        }
        /// <summary>
        /// 判断httpmethod是否可使用该节点
        /// </summary>
        /// <param name="httpmethod"></param>
        /// <returns></returns>
        public bool JudgeHttpMethod(string httpmethod)
        {
            if (_allowHttpMethod == 0) return true;
            httpmethod = httpmethod.ToLower();
            switch (httpmethod)
            {
                case "get":
                    return (_allowHttpMethod & (byte)EHttpMethod.Get) != 0;
                case "post":
                    return (_allowHttpMethod & (byte)EHttpMethod.Post) != 0;
                case "put":
                    return (_allowHttpMethod & (byte)EHttpMethod.Put) != 0;
                case "delete":
                    return (_allowHttpMethod & (byte)EHttpMethod.Delete) != 0;
                default:
                    return false;
            }
        }
        public ActionNode(PluginObject plg, string controller, MethodInfo method)
        {
            _allowHttpMethod = 0;
            mKey = method.Name;
            mMethod = method;
   
            string taboutmodule = string.Format("/{0}/{1}", plg.Name, controller);
            string taboutaction = string.Empty;//默认关联控制器权限

            //判断是否为异步
            _async = method.ReturnType != typeof(void) && typeof(Task).IsAssignableFrom(method.ReturnType);

            //描述特性
            DesAttribute ad = (DesAttribute)method.GetCustomAttribute(typeof(DesAttribute));
            if (ad != null)
            {
                mDescript = ad.Des;
                mSort = ad.Sort;
                taboutaction = mKey;
            }

            //关联特性
            AboutAttribute ab = (AboutAttribute)method.GetCustomAttribute(typeof(AboutAttribute));
            if (ab != null)
            {
                if (!string.IsNullOrEmpty(ab.AbountModule))
                {
                    taboutmodule = ab.AbountModule;
                }
                if (!string.IsNullOrEmpty(ab.AbountAction))
                {
                    taboutaction = ab.AbountAction;
                }
            }


            //路由特性
            RouteAttribute rtattr = (RouteAttribute)method.GetCustomAttribute(typeof(RouteAttribute));
            if (rtattr != null)
            {
                //添加插件路由
                plg.Data.Get<IPluginRouterBuilder>()?.AddRouter(plg.Name, controller, mKey, rtattr.Template);
            }

  
            Attribute getattr = method.GetCustomAttribute(typeof(HttpGetAttribute));
            if (getattr != null)
            {
                _allowHttpMethod |= (byte)EHttpMethod.Get;
            }

            Attribute postattr = method.GetCustomAttribute(typeof(HttpPostAttribute));
            if (postattr != null)
            {
                _allowHttpMethod |= (byte)EHttpMethod.Post;
            }

            Attribute putattr = method.GetCustomAttribute(typeof(HttpPutAttribute));
            if (putattr != null)
            {
                _allowHttpMethod |= (byte)EHttpMethod.Put;
            }

            Attribute delattr = method.GetCustomAttribute(typeof(HttpDeleteAttribute));
            if (delattr != null)
            {
                _allowHttpMethod |= (byte)EHttpMethod.Delete;
            }


            mAboutModule = taboutmodule;
            mAboutAction = taboutaction;
        }
    }
}
