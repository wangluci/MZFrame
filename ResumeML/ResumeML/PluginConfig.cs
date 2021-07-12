using ResumeML.Business;
using TemplateAction.Core;

namespace ResumeML
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services, IEventRegister register)
        {
            services.AddSingleton<TestBusiness, TestBusiness>((object[] arguments) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(TestBusiness), arguments);
            });
        }
        public void Loaded(ITAApplication app) { }
        public void Unload() { }
    }
}
