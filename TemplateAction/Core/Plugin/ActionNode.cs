using System.Reflection;

namespace TemplateAction.Core
{
    /// <summary>
    /// 动作树节点
    /// </summary>
    public class ActionNode : Node
    {
        protected MethodInfo mMethod;
        public MethodInfo Method
        {
            get { return mMethod; }
        }
        protected AsyncAttribute _async;
        /// <summary>
        /// 判断是否为异步方法
        /// </summary>
        public AsyncAttribute Async
        {
            get { return _async; }
        }
        /// <summary>
        /// 关联模块
        /// </summary>
        protected string mAboutModule;
        public string AboutModule
        {
            get { return mAboutModule; }
        }

        /// <summary>
        /// 关联动作
        /// </summary>
        protected string mAboutAction;
        public string AboutAction
        {
            get { return mAboutAction; }
        }

        protected bool _isdes;
        public bool IsDes
        {
            get { return _isdes; }
        }
        protected byte _allowHttpMethod;
        public byte AllowHttpMethod
        {
            get { return _allowHttpMethod; }
        }
        protected enum EHttpMethod : byte
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
        public ActionNode(PluginObject plugin, string controller, MethodInfo method)
        {
            _allowHttpMethod = 0;
            mKey = method.Name;
            mMethod = method;
            bool allowget = false;
            bool allowput = false;
            bool allowpost = false;
            bool allowdel = false;
            bool initabout = false;
            bool initroute = false;

            string tdesact = "";
            string taboutmodule = string.Format("/{0}/{1}", plugin.Name, controller);
            string taboutaction = mKey;
            _isdes = false;
            object[] acattrs = method.GetCustomAttributes(false);
            foreach (object attrobj in acattrs)
            {
                if (!_isdes)
                {
                    DesAttribute ad = attrobj as DesAttribute;
                    if (ad != null)
                    {
                        tdesact = ad.Des;
                        _isdes = true;
                    }
                }
                if (!initabout && !_isdes)
                {
                    AboutAttribute ab = attrobj as AboutAttribute;
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
                        _isdes = true;
                        initabout = true;
                    }
                }
                if (!initroute)
                {
                    RouteAttribute rtattr = attrobj as RouteAttribute;
                    if (rtattr != null)
                    {
                        //添加插件路由
                        plugin.RouterBuilder.AddRouter(plugin.Name, controller, mKey, rtattr.Template);
                        initroute = true;
                    }
                }
                if (_async == null)
                {
                    AsyncAttribute rtattr = attrobj as AsyncAttribute;
                    if (rtattr != null)
                    {
                        _async = rtattr;
                    }
                }
                if (!allowget)
                {
                    HttpGetAttribute getattr = attrobj as HttpGetAttribute;
                    if (getattr != null)
                    {
                        _allowHttpMethod |= (byte)EHttpMethod.Get;
                        allowget = true;
                    }
                }
                if (!allowpost)
                {
                    HttpPostAttribute postattr = attrobj as HttpPostAttribute;
                    if (postattr != null)
                    {
                        _allowHttpMethod |= (byte)EHttpMethod.Post;
                        allowpost = true;
                    }
                }
                if (!allowput)
                {
                    HttpPutAttribute putattr = attrobj as HttpPutAttribute;
                    if (putattr != null)
                    {
                        _allowHttpMethod |= (byte)EHttpMethod.Put;
                        allowput = true;
                    }
                }
                if (!allowdel)
                {
                    HttpDeleteAttribute delattr = attrobj as HttpDeleteAttribute;
                    if (delattr != null)
                    {
                        _allowHttpMethod |= (byte)EHttpMethod.Delete;
                        allowdel = true;
                    }
                }
            }

            mDescript = tdesact;
            mAboutModule = taboutmodule;
            mAboutAction = taboutaction;

        }
    }
}
