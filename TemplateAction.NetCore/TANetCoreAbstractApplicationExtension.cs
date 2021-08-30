using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public static class TANetCoreAbstractApplicationExtension
    {
        /// <summary>
        /// 微软内置服务转移到TA的服务中
        /// </summary>
        /// <param name="app"></param>
        /// <param name="tservices"></param>
        /// <returns></returns>
        public static void CopyServicesFrom(this TAAbstractApplication app, Microsoft.Extensions.DependencyInjection.ServiceCollection tservices)
        {
            //映射服务
            app.Services.AddSingleton<IServiceProvider, TANetServiceProvider>();

            //复制服务
            foreach (Microsoft.Extensions.DependencyInjection.ServiceDescriptor micsd in tservices)
            {
                ServiceLifetime lifetime = ServiceLifetime.Singleton;
                switch (micsd.Lifetime)
                {
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped:
                        lifetime = ServiceLifetime.Scope;
                        break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient:
                        lifetime = ServiceLifetime.Transient;
                        break;
                    case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton:
                        lifetime = ServiceLifetime.Singleton;
                        break;
                }
                ProxyFactory pfactory = null;
                if (micsd.ImplementationFactory != null)
                {
                    pfactory = (object[] constructorArguments) =>
                    {
                        return micsd.ImplementationFactory.Invoke(app.ServiceProvider.GetService<IServiceProvider>());
                    };
                }
                app.Services.Add(micsd.ServiceType.FullName, new ServiceDescriptor(micsd.ImplementationType, lifetime, pfactory, micsd.ImplementationInstance));
            }
        }
    }
}
