using System;
using TemplateAction.Core;

namespace AuthService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
        }
        public void Loaded(ITAApplication app, IEventRegister register) { }
        public void Unload() { }
    }
}
