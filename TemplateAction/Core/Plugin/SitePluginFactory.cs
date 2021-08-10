using System.Reflection;

namespace TemplateAction.Core
{
    public class SitePluginFactory : IPluginFactory
    {
        internal IRouterCollection RouterCollection { get; set; }
        /// <summary>
        /// 路由创建工厂
        /// </summary>
        internal IRouterBuilder RouterBuilder { get; set; }
        public PluginObject Create(Assembly tAssembly, string filepath)
        {
            PluginObject newObj = new PluginObject(this, new SiteExtentionData(), tAssembly, filepath);
            if (newObj.Config == null) return null;
            return newObj;
        }
    }
}
