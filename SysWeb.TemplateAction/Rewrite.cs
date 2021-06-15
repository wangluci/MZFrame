using System;
using System.Web;
using TemplateAction.Core;
using TemplateAction.Common;
using System.Configuration;
using TemplateAction.Core.Dispatcher;
using TemplateAction.Core.Extensions;

namespace SysWeb.TemplateAction
{
    /// <summary>
    /// mvc的重定向类
    /// </summary>
    public class Rewrite : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            string s = ConfigurationManager.AppSettings["RewriteNoRoot"] ?? "false";
            _noRoot = Convert.ToBoolean(s);
            InitRewrite(context);
        }
        public void InitRewrite(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Syn_BeginRequest);
            context.AddOnBeginRequestAsync(ASyn_BeginRequest, EndEventHandler);
        }
        /// <summary> 
        /// 实现接口的Dispose方法 
        /// </summary> 
        public void Dispose()
        {
        }
        private bool _noRoot = false;
        /// <summary>
        /// 同步请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Syn_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            try
            {
                HttpContext context = application.Context;
                string requestUrl = context.Request.Path;
                if (_noRoot && requestUrl.Equals("/"))
                {
                    return;
                }
                if (requestUrl.EndsWith(TAUtility.FILE_EXT, StringComparison.OrdinalIgnoreCase))
                {
                    application.Response.ContentType = "application/json";
                    application.Response.Write("{\"Code\":-667,\"Message\":\"文件受限制不能访问\"}");
                    application.CompleteRequest();
                    return;
                }
                if (TAUtility.IsStaticFile(requestUrl))
                {
                    return;
                }

                SysWebContext syscontext = new SysWebContext(context);
                TAEventDispatcher.Instance.Dispatch(new ContextCreatedEvent(syscontext));
                TARequestHandleBuilder builder = syscontext.Application.UseRoute(syscontext);
                if (builder == null)
                {
                    application.Response.ContentType = "application/json";
                    application.Response.Write("{\"Code\":-669,\"Message\":\"请先配置路由\"}");
                    application.CompleteRequest();
                    return;
                }
                if (builder.CreateAsync())
                {
                    context.Items["Async"] = builder;
                    return;
                }
                else
                {
                    builder.BuildAndExcute().Output();
                }
                application.CompleteRequest();
            }
            catch (Exception ex)
            {
                application.Response.ContentType = "application/json";
                application.Response.Write("{\"Code\":-666,\"Message\":\"" + ex.Message.Replace("\"", "") + "\"}");
                application.CompleteRequest();
            }

        }
        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="cb"></param>
        /// <param name="extraData"></param>
        public IAsyncResult ASyn_BeginRequest(Object sender, EventArgs e, AsyncCallback cb, Object extraData)
        {
            HttpApplication application = (HttpApplication)sender;
            TARequestHandleBuilder builder = application.Context.Items["Async"] as TARequestHandleBuilder;
            if (builder == null)
            {
                IAsyncResult errar = new ErrAsyncResult();
                cb(errar);
                return errar;
            }
            return new SysWebAsyncResult(cb, builder);
        }
        /// <summary>
        /// 服务端超时或有结果时执行
        /// </summary>
        /// <param name="ar"></param>
        public void EndEventHandler(IAsyncResult ar)
        {
        }
    }
}
