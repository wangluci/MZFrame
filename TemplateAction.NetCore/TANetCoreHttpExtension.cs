using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;
using TemplateAction.Route;

namespace TemplateAction.NetCore
{
    public static class TANetCoreHttpExtension
    {
        public static void Write(this HttpResponse response, string content)
        {
            using (StreamWriter sw = new StreamWriter(response.Body))
            {
                sw.Write(content);
            }
        }
        public static void Write(this HttpResponse response, byte[] content)
        {
            using (BinaryWriter sw = new BinaryWriter(response.Body))
            {
                sw.Write(content);
            }
        }
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, MyAccess.Json.Json.Encode(value));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            string value = session.GetString(key);
            return value == null ? default(T) : MyAccess.Json.Json.DecodeType<T>(value);
        }
        /// <summary>
        /// 文件新增异步保存
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this IRequestFile file, string filename)
        {
            await ((TANetCoreHttpFile)file).SaveAsAsync(filename);
        }
        /// <summary>
        /// 微软内置服务转移到TA的服务中
        /// </summary>
        public static IApplicationBuilder ServicesMoveToApp(this IApplicationBuilder appBuilder, TASiteApplication app)
        {                    
            //映射服务
            app.Services.AddSingleton<IServiceProvider, TANetServiceProvider>();

            //复制服务
            Microsoft.Extensions.DependencyInjection.ServiceCollection tservices = appBuilder.ServerFeatures.Get<Microsoft.Extensions.DependencyInjection.ServiceCollection>();
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
                app.Services.Add(micsd.ImplementationType.FullName, new ServiceDescriptor(micsd.ServiceType, lifetime, pfactory, micsd.ImplementationInstance));
            }
            return appBuilder;
        }

        public static IApplicationBuilder UseTAMvc(this IApplicationBuilder appBuilder, Action<TASiteApplication> init = null)
        {
            if (init == null)
            {
                TAEventDispatcher.Instance.RegisterLoadBefore<TASiteApplication>(app =>
                {
                    appBuilder.ServicesMoveToApp(app);

                    //设置路由
                    string defns = null;
                    Assembly ass = Assembly.GetEntryAssembly();
                    if (ass != null)
                    {
                        defns = ass.GetName().Name;
                    }
                    RouterBuilder rtbuilder = new RouterBuilder();
                    rtbuilder.UsePlugin();
                    if (defns != null)
                    {
                        rtbuilder.UseDefault(defns);
                    }
                    app.UseRouterBuilder(rtbuilder);

                });
            }
            else
            {
                TAEventDispatcher.Instance.RegisterLoadBefore(init);
            }

            appBuilder.Use(next =>
            {
                return context =>
                {
                    TANetCoreHttpApplication taapp = context.Features.Get<TANetCoreHttpApplication>();
                    if (taapp != null)
                    {
                        string requestUrl = context.Request.Path;
                        if (requestUrl.EndsWith(TAUtility.FILE_EXT, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.Write("{\"Code\":-667,\"Message\":\"文件受限制不能访问\"}");
                            return Task.CompletedTask;
                        }
                        else if (TAUtility.IsStaticFile(requestUrl))
                        {
                            return next(context);
                        }
                        TANetCoreHttpContext tacontext = new TANetCoreHttpContext(taapp, context);
                        TAActionBuilder builder = tacontext.Application.Route(tacontext);
                        if (builder != null)
                        {
                            if (builder.Async)
                            {
                                return builder.StartAsync();
                            }
                            else
                            {
                                builder.Start().Output();
                                return Task.CompletedTask;
                            }
                        }
                  
                    }
                    return next(context);
                };
            });
            return appBuilder;
        }

    }
}
