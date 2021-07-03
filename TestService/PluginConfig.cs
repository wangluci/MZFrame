using System;
using TemplateAction.Core;

namespace TestService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services, IEventDispatcher dispatcher)
        {
            //新增一个事件监听
            dispatcher.AddListener(new TestListener());
            //注册一个事件分发
            services.AddSingleton<ITestListener>((object[] arguments) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDispatcher<ITestListener>(TAEventDispatcher.Instance.Dispatch);
            });
        }
        public void Unload() { }
    }
}
