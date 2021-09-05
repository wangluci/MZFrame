using System;
using TemplateAction.Core;
using TemplateAction.Extension.Site;

namespace TestService
{
    public class PluginConfig : IPluginConfig
    {
        public void Configure(IServiceCollection services)
        {
            //注册一个事件分发
            services.AddSingleton<ITestListener>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDispatcher<ITestListener>(TAEventDispatcher.Instance.Dispatch);
            });
            //添加身份验证
            services.AddSingleton<AuthMiddleware>();
            //添加监听实例
            services.AddSingleton<TestListener>();
            //添加服务
            services.AddSingleton<TestService>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(TestService), arguments);
            });
            //注册异步DAL
            services.AddSingleton<TestDALAsync>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(TestDALAsync), arguments);
            });
            //注册同步DAL
            services.AddSingleton<TestDAL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(TestDAL), arguments);
            });

        }
        public void Loaded(ITAApplication app, IEventRegister register)
        {
            //使用身份认证
            app.UseMiddlewareFirst<AuthMiddleware>();
            //新增一个事件监听
            register.AddListener(app.ServiceProvider.GetService<TestListener>());
        }
        public void Unload() { }
    }
}
