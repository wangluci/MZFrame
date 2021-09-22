using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ML;
using ResumeMLML.Model;
using System;
using System.IO;
using System.Reflection;
using TemplateAction.NetCore;
using TemplateAction.Route;
using TemplateAction.Core;
using Microsoft.Extensions.DependencyInjection;
using Common.Redis;

namespace ResumeML
{
    class Program
    {
        static void Main(string[] args)
        {
            string tpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "log4net.config";
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(tpath));
            PanGu.Segment.Init();
            TANetCoreHttpHostBuilder.CreateDefaultHostBuilder().ConfigureServices(services =>
            {
                services.AddPredictionEnginePool<ModelInput, ModelOutput>()
    .FromFile(modelName: "ResumeModel", filePath: "MLModel.zip", watchForChanges: true);
            }).Configure((IApplicationBuilder builder) =>
            {
                IConfiguration config = builder.ApplicationServices.GetService<IConfiguration>();
                builder.UseAllowCORS();
                builder.UseStaticFiles();
                builder.UseTAMvc(app =>
                {
                    //注册字符串
                    app.Services.AddString("connstr", config.GetSection("connstr").Value);
                    app.Services.AddSingleton<RedisHelper>(new RedisHelper(config.GetSection("redisconn").Value));
                    RouterBuilder rbuilder = new RouterBuilder();
                    rbuilder.MapRoute("AuthService", "vue-element-admin/{controller=Home}/{action=Index}/{id?}");
                    rbuilder.UseDefault(Assembly.GetEntryAssembly().GetName().Name);
                    app.UseRouterBuilder(rbuilder);
                });
            }).Build().Run();
        }

    }
}
