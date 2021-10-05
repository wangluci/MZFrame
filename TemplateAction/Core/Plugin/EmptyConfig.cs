

namespace TemplateAction.Core
{
    /// <summary>
    /// 空配置文件
    /// </summary>
    public class EmptyConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
        }

        public void Loaded(ITAApplication app, IEventRegister register)
        {
        }

        public void Unload()
        {
        }
    }
}
