using System;
using TemplateAction.Core;

namespace TestService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services, IEventDispatcher dispatcher)
        {
        }
        public void Unload() { }
    }
}
