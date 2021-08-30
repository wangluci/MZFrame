using System.Reflection;

namespace TemplateAction.Core
{
    public class SitePluginCollectionExtData : IPluginCollectionExtData
    {
        internal IRouterCollection RouterCollection { get; set; }
        /// <summary>
        /// 路由创建工厂
        /// </summary>
        internal IRouterBuilder RouterBuilder { get; set; }
        public IExtentionData CreateExtentionData()
        {
            return new SiteExtentionData();
        }
    }
}
