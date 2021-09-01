using System;
using System.IO;
using TemplateAction.Core;
using TemplateAction.Route;
using TemplateAction.Extension;
using System.Web;
using System.Configuration;
namespace MainWeb
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            TAEventDispatcher.Instance.RegisterLoadBefore<TASiteApplication>(app =>
            {
                //设置路由
                app.UseRouterBuilder(new RouterBuilder().UsePlugin().UseDefault("TestService"));
                //注册字符串
                app.Services.AddString("testconnstr", ConfigurationManager.AppSettings["testconnstr"]);
            });
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Server.MapPath("log4net.config")));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}