using System;
using TemplateAction.Core;

namespace TestService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            //注册一个事件分发
            services.AddSingleton<ITestListener>((object[] arguments) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDispatcher<ITestListener>(TAEventDispatcher.Instance.Dispatch);
            });
            //添加身份验证
            services.AddSingleton<AuthMiddleware>();
            //添加监听实例
            services.AddSingleton<TestListener>();
        }
        public void Loaded(ITAApplication app, IEventRegister register) {
            //新增一个事件监听
            register.AddListener(app.ServiceProvider.GetService<TestListener>());
        }
        public void Unload() { }
    }
}
