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
        /// 设置使用TA的MVC
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTAMvc(this IApplicationBuilder appBuilder, Action<TASiteApplication> init = null)
        {
            if (init == null)
            {
                TAEventDispatcher.Instance.RegisterLoadBefore<TASiteApplication>(app =>
                {
                    Microsoft.Extensions.DependencyInjection.ServiceCollection tservices = appBuilder.ServerFeatures.Get<Microsoft.Extensions.DependencyInjection.ServiceCollection>();
                    app.CopyServicesFrom(tservices);

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
