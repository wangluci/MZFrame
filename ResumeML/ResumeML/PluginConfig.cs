using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace ResumeML
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services, IEventDispatcher dispatcher)
        {
        }
        public void Unload() { }
    }
}
