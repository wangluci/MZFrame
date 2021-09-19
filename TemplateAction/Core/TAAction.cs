using System;
using System.Threading;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 代表Action
    /// </summary>
    public class TAAction : AbstractTemplateContext, ITAAction, ILifetimeFactory
    {

        private string mNameSpace;
        public string NameSpace
        {
            get { return mNameSpace; }
        }
        private string mController;
        public string Controller
        {
            get { return mController; }
        }
        private string mAction;
        public string Action
        {
            get { return mAction; }
        }
        private ITAContext mContext;
        public ITAContext Context
        {
            get
            {
                return mContext;
            }
        }
        private ITemplateContext mTemplateContext;
        public ITemplateContext TemplateContext { get { return mTemplateContext; } }

        private ITAObjectCollection _extparams;
        public ITAObjectCollection ExtParams
        {
            get { return _extparams; }
        }
        private ControllerNode _controllerNode;
        public ControllerNode ControllerNode
        {
            get { return _controllerNode; }
        }
        private ActionNode _node;
        public ActionNode ActionNode
        {
            get { return _node; }
        }
        /// <summary>
        /// 获取或设置当前请求的异常处理
        /// </summary>
        public Func<Exception, IResult> ExceptionFun { get; internal set; }
        private static AsyncLocal<TAAction> _current = new AsyncLocal<TAAction>();
        /// <summary>
        /// 当前Action
        /// </summary>
        public static TAAction Current
        {
            get
            {
                return _current.Value;
            }
        }
        public TAAction(ITAContext context, ControllerNode controller, ActionNode action, ITAObjectCollection ext)
        {
            mContext = context;
            mTemplateContext = this;
            _extparams = ext;
            _controllerNode = controller;
            _node = action;
            mNameSpace = controller.PluginName;
            mController = controller.Key;
            mAction = action.Key;
            _current.Value = this;
        }
        public void AddGlobal(string key, object value)
        {
            PushGlobal(key, value);
        }
        public T Global<T>(string key, T def)
        {
            return GetGlobal(key, def);
        }
        public bool IsDefine(string key)
        {
            return mGlobalReplace.ContainsKey(key);
        }
        /// <summary>
        /// 执行Action拦截器中间件
        /// </summary>
        /// <returns></returns>
        public object Excute()
        {
            return mContext.Application.Filters.Excute(this);
        }
        /// <summary>
        /// 模板Include语法调用
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public override string Include(string src)
        {
            string zPath = string.Empty;
            if (src.StartsWith("/"))
            {
                zPath = src;
            }
            else
            {
                zPath = "/" + mNameSpace + "/" + mController + "/" + src;
            }
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(zPath);
            if (indexTemp == null)
            {
                return "视图不存在";
            }
            return indexTemp.MakeHtml(this);
        }
        private const string CONTROLLER_PRE = "CONTR_STORE_$$";
        public object GetValue(IInstanceFactory instanceFactory, Type serviceType, ProxyFactory factory)
        {
            string tkey = CONTROLLER_PRE + serviceType.FullName;
            if (mContext.Items.Contains(tkey))
            {
                return mContext.Items[tkey];
            }
            else
            {
                object target = instanceFactory.CreateServiceInstance(serviceType, factory, this);
                mContext.Items[tkey] = target;
                return target;
            }
        }
    }
}
