using System;
namespace TemplateAction.Core
{
    public interface ITAApplication
    {
        string RootPath { get; }
        string PluginPath { get; }
        IServiceCollection Services { get; }
        ITAServices ServiceProvider { get; }
        /// <summary>
        /// 获取指定插件的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ns"></param>
        /// <returns></returns>
        T FindConfig<T>(string ns) where T : IPluginConfig;

        /// <summary>
        /// 判断是否包含指定名称的插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        bool PluginExist(string ns);
    }
}
