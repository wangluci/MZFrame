using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace ShareService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            //添加共享实例
            services.AddSingleton<ShareInstance>();
        }
        public void Loaded(ITAApplication app, IEventRegister register)
        {
        }
        public void Unload() { }
    }
}
