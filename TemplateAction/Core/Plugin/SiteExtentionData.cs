using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class SiteExtentionData : IExtentionData
    {
        /// <summary>
        /// 路由创建工厂
        /// </summary>
        private IPluginRouterBuilder _routerBuilder;
        internal IPluginRouterBuilder RouterBuilder
        {
            get { return _routerBuilder; }
        }
        private IRouterCollection _routerCollection;
        public IRouterCollection RouterCollection
        {
            get { return _routerCollection; }
        }
        private Dictionary<string, ControllerNode> mControllerList;
        public Dictionary<string, ControllerNode> ControllerList 
        { 
            get { return mControllerList; } 
        }
        private string _myControllerName;
        public void LoadBefore(IPluginCollectionExtData factory, Assembly assembly, string pluginpath)
        {
            SitePluginCollectionExtData siteFactory = (SitePluginCollectionExtData)factory;
            if (siteFactory.RouterBuilder != null)
            {
                this._routerBuilder = siteFactory.RouterBuilder.NewPluginBuilder();
            }
            this.mControllerList = new Dictionary<string, ControllerNode>();
            this._myControllerName = typeof(IController).FullName;
        }
        public bool LoadItem(string pluginName, Type t)
        {
            if (t.GetInterface(this._myControllerName) != null)
            {
                //获取插件的控制器节点数据
                ControllerNode n = new ControllerNode(pluginName, this, t);
                this.mControllerList[n.Key.ToLower()] = n;
                return true;
            }
            return false;
        }
        public void LoadAfter(IPluginCollectionExtData factory, Assembly assembly, string pluginpath)
        {
            if (this._routerBuilder != null)
            {
                this._routerCollection = this._routerBuilder.Build();
            }
        }
    }
}
