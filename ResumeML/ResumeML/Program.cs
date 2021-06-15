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
            MyAccess.WordSegment.Segment.Init();
            TANetCoreHttpHostBuilder.CreateDefaultHostBuilder().Build().Run();
        }

    }
}
