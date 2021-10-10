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
                builder.UseAllowCORS();
                builder.UseStaticFiles();
                builder.UseTAMvc(app =>
                {
                    app.UseParamMapping(new BodyParamMapping(json: (json, t) =>
                     {
                         return MyAccess.Json.Json.Decode(json, t);
                     }));
                    app.UseRouterBuilder(new RouterBuilder().UsePlugin().UseDefault(Assembly.GetEntryAssembly().GetName().Name));
                });
            }).Build().Run();
        }

    }
}
