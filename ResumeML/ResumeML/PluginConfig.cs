using ResumeML.Business;
using TemplateAction.Core;
namespace ResumeML
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<TestBusiness>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(TestBusiness), arguments);
            });


            services.AddSingleton<FieldDict>();
        }
        public void Loaded(ITAApplication app, IEventRegister register) { }
        public void Unload() { }
    }
}
