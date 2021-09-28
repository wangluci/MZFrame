using System;
namespace TemplateAction.Core
{
    public interface ITAApplication
    {
        /// <summary>
        /// 应用根目录
        /// </summary>
        string RootPath { get; }
        /// <summary>
        /// 插件目录
        /// </summary>
        string PluginPath { get; }
        /// <summary>
        /// 服务提供
        /// </summary>
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
        bool ExistPlugin(string ns);
        /// <summary>
        /// 处理定时同步任务
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="ts"></param>
        void PushConcurrentTask(Action ac, TimeSpan ts);
    }
}
