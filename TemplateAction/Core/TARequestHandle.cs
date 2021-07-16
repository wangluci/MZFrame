using System;
using System.Collections.Generic;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TARequestHandle : AbstractTemplateContext, IRequestHandle
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
        protected Type _controllerType;
        public Type ControllerType
        {
            get { return _controllerType; }
        }
        protected ActionNode _node;
        public ActionNode ActionNode
        {
            get { return _node; }
        }

        public TARequestHandle(PluginCollection pluginCollection, ITAContext context, string ns, string controller, string action, ITAObjectCollection ext)
        {
            mContext = context;
            mTemplateContext = this;
            _extparams = ext;
            string tmpns = ns.ToLower();
            string tmpct = controller.ToLower();
            string tmpac = action.ToLower();
            _controllerType = pluginCollection.GetControllerByKeyInPlugin(tmpns, tmpct);
            _node = pluginCollection.GetMethodByKeyInPlugin(tmpct, tmpac, tmpns);
            mNameSpace = _controllerType.Assembly.GetName().Name;
            mController = _controllerType.Name;
            mAction = _node.Method.Name;
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
            string realpath = TemplateApp.Instance.Relative2TemplatePath(zPath);
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(realpath);
            if (indexTemp == null)
            {
                return "视图不存在";
            }
            return indexTemp.MakeHtml(this);
        }
    }
}
