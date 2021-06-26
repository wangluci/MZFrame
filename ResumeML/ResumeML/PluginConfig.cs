using ResumeML.Business;
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
            services.AddSingleton<TestBusiness, TestBusiness>((object[] arguments) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(TestBusiness), arguments);
            });
        }
        public void Unload() { }
    }
}
