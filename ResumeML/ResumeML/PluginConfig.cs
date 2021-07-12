using ResumeML.Business;
using TemplateAction.Core;

namespace ResumeML
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<TestBusiness, TestBusiness>((object[] arguments) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(TestBusiness), arguments);
            });
        }
        public void Loaded(ITAApplication app, IEventRegister register) { }
        public void Unload() { }
    }
}
