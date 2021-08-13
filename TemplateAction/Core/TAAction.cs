using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 代表Action
    /// </summary>
    public class TAAction : AbstractTemplateContext, ITAAction, ILifetimeFactory
    {

        protected string mNameSpace;
        public string NameSpace
        {
            get { return mNameSpace; }
        }
        protected string mController;
        public string Controller
        {
            get { return mController; }
        }
        protected string mAction;
        public string Action
        {
            get { return mAction; }
        }
        protected ITAContext mContext;
        public ITAContext Context
        {
            get
            {
                return mContext;
            }
        }
        protected ITemplateContext mTemplateContext;
        public ITemplateContext TemplateContext { get { return mTemplateContext; } }

        protected ITAObjectCollection _extparams;
        public ITAObjectCollection ExtParams
        {
            get { return _extparams; }
        }
        protected ControllerNode _controllerNode;
        public ControllerNode ControllerNode
        {
            get { return _controllerNode; }
        }
        protected ActionNode _node;
        public ActionNode ActionNode
        {
            get { return _node; }
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
        protected const string CONTROLLER_PRE = "CONTR_STORE_$$";
        public object GetValue(PluginCollection collection, Type serviceType, ProxyFactory factory, ILifetimeFactory extFactory)
        {
            string tkey = CONTROLLER_PRE + serviceType.FullName;
            if (mContext.Items.Contains(tkey))
            {
                return mContext.Items[tkey];
            }
            else
            {
                object target = collection.CreateServiceInstance(serviceType, factory, extFactory);
                mContext.Items[tkey] = target;
                return target;
            }
        }
    }
}
