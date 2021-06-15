using System;
namespace TemplateAction.Core
{
    public interface IPluginRouterBuilder: IRouterCollectionBuilder
    {
        /// <summary>
        /// 创建插件路由
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        void AddRouter(string ns, string controller, string action, string template);
    }
}
