
namespace TemplateAction.Core
{
    public interface IRouterBuilder: IRouterCollectionBuilder
    {
        /// <summary>
        /// 创建插件路由Builder
        /// </summary>
        /// <returns></returns>
        IPluginRouterBuilder NewPluginBuilder();
    }
}
