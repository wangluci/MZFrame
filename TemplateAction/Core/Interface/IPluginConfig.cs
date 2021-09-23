namespace TemplateAction.Core
{
    /// <summary>
    /// 插件配置接口
    /// 每个插件加载时，会先执行此配置
    /// </summary>
    public interface IPluginConfig
    {

        /// <summary>
        /// 注册插件的服务和事件
        /// </summary>
        /// <param name="services"></param>
        void Configure(IServiceCollection services);
        /// <summary>
        /// 插件加载完成后调用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="register"></param>
        void Loaded(ITAApplication app, IEventRegister register);
        /// <summary>
        /// 插件御载处理
        /// </summary>
        void Unload();
    }
}
