using System;
using TemplateAction.Core;

namespace AuthService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<AuthBLL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(AuthBLL), arguments);
            });
            services.AddSingleton<AuthDAL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(AuthDAL), arguments);
            });
            services.AddSingleton<PermissionDAL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(PermissionDAL), arguments);
            });
        }
        public void Loaded(ITAApplication app, IEventRegister register) { }
        public void Unload() { }
    }
}
