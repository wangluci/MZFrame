using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.ML;
using ResumeMLML.Model;
using System;
using System.IO;
using System.Reflection;
using TemplateAction.NetCore;
using TemplateAction.Route;

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
                builder.UseStaticFiles();
                builder.UseTAMvc(app =>
                {
                    string defns = Assembly.GetEntryAssembly().GetName().Name;
                    RouterBuilder rbuilder = new RouterBuilder();
                    rbuilder.MapRoute(defns, "vue-element-admin/{controller=Home}/{action=Index}/{id?}");
                    rbuilder.UseDefault(defns);
                    app.UseRouterBuilder(rbuilder);
                });
            }).Build().Run();
        }

    }
}
