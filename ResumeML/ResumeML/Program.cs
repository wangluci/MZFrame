using Microsoft.Extensions.ML;
using ResumeMLML.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.NetCore;

namespace ResumeML
{
    class Program
    {
        static void Main(string[] args)
        {
            string tpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "log4net.config";
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(tpath));
            PanGu.Segment.Init();
            TANetCoreHttpHostBuilder.CreateDefaultHostBuilder().ConfigureServices(services=> {
                services.AddPredictionEnginePool<ModelInput, ModelOutput>()
    .FromFile(modelName: "ResumeModel", filePath: "MLModel.zip", watchForChanges: true);
            }).Build().Run();
        }

    }
}
