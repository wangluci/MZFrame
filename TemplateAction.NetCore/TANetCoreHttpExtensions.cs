using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;

namespace TemplateAction.NetCore
{
    public static class TANetCoreHttpExtensions
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
        public static IApplicationBuilder UseTAMvc(this IApplicationBuilder app, Action<TASiteApplication> init = null)
        {
            if (init != null)
            {
                TAEventDispatcher.Instance.RegisterLoadBefore(init);
            }

            app.Use(next =>
            {
                return context =>
                {
                    TANetCoreHttpApplication taapp = app.ApplicationServices.GetService(typeof(TANetCoreHttpApplication)) as TANetCoreHttpApplication;
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
            return app;
        }

    }
}
