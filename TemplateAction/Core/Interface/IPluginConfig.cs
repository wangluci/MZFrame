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
        /// <param name="register"></param>
        void Configure(IServiceCollection services, IEventRegister register);
        /// <summary>
        /// 插件御载处理
        /// </summary>
        void Unload();
    }
}
