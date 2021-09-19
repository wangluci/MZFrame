using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public class SitePluginCollectionExtData : IPluginCollectionExtData
    {
        internal IRouterCollection RouterCollection { get; set; }
        /// <summary>
        /// 路由创建工厂
        /// </summary>
        internal IRouterBuilder RouterBuilder { get; set; }
        private static string ControllerInterfaceName = typeof(IController).FullName;
        public void PluginLoadBefore(PluginObject plg)
        {
            if (this.RouterBuilder != null)
            {
                plg.Data.Set<IPluginRouterBuilder>(this.RouterBuilder.NewPluginBuilder());
            }
            plg.Data.Set(new Dictionary<string, ControllerNode>());
        }
        public bool PluginLoadType(PluginObject plg, Type t)
        {
            if (t.GetInterface(ControllerInterfaceName) != null)
            {
                //获取插件的控制器节点数据
                ControllerNode n = new ControllerNode(plg, t);
                plg.Data.Get<Dictionary<string, ControllerNode>>()[n.Key.ToLower()] = n;
                return true;
            }
            return false;
        }
        public void PluginLoadAfter(PluginObject plg)
        {
            IPluginRouterBuilder plgRouterBuilder = plg.Data.Get<IPluginRouterBuilder>();
            if (plgRouterBuilder != null)
            {
                plg.Data.Set<IRouterCollection>(plgRouterBuilder.Build());
            }
        }
    }
}
