using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpApplication : TAApplication, IHttpApplication<HttpContext>
    {
        private readonly RequestDelegate _requestDelegate;
        private IApplicationBuilder _appbuilder;

        public TANetCoreHttpApplication(IApplicationBuilder appbuilder)
        {
            _requestDelegate = appbuilder.Build();
            _appbuilder = appbuilder;
        }

        public HttpContext CreateContext(IFeatureCollection contextFeatures)
        {
            DefaultHttpContext df = new DefaultHttpContext(contextFeatures);
            df.RequestServices = _appbuilder.ApplicationServices;
            return df;
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
        }

        public async Task ProcessRequestAsync(HttpContext context)
        {
            await _requestDelegate(context);
        }
    }
}
