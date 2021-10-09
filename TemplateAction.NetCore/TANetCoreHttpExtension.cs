using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
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
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
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
        /// 允许CORS请求
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAllowCORS(this IApplicationBuilder appBuilder)
        {
            appBuilder.Use(next =>
            {
                return context =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                        context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        context.Response.Headers.Add("Access-Control-Allow-Headers", "authorization, content-type,Cookie");
                        return Task.CompletedTask;
                    }
                    return next(context);
                };
            });
            return appBuilder;
        }

        /// <summary>
        /// 设置使用TA的MVC
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTAMvc(this IApplicationBuilder appBuilder, Action<TASiteApplication> init = null)
        {
            if (init == null)
            {
                init = app =>
                {
                    //设置路由
                    string defns = Assembly.GetEntryAssembly().GetName().Name;
                    app.UseRouterBuilder(new RouterBuilder().UsePlugin().UseDefault(defns));
                };
            }
            TAEventDispatcher.Instance.RegisterLoadBefore<TASiteApplication>(app =>
            {
                Microsoft.Extensions.DependencyInjection.ServiceCollection tservices = appBuilder.ServerFeatures.Get<Microsoft.Extensions.DependencyInjection.ServiceCollection>();
                app.Services.CopyServicesFrom(tservices);
                init(app);
            });

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
