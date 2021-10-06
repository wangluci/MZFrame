using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpApplication : TASiteApplication, IHttpApplication<HttpContext>
    {
        private readonly RequestDelegate _requestDelegate;
        private IApplicationBuilder _appbuilder;
        private Dictionary<string, AssemblyLoadContext> _assemblyContexts = new Dictionary<string, AssemblyLoadContext>();
        private bool _useUnload = false;
        
        /// <summary>
        /// 是否允许释放插件内存
        /// </summary>
        /// <returns></returns>
        public TANetCoreHttpApplication UseMemoryUnload()
        {
            if (!_useUnload)
            {                
                _useUnload = true;
            }
            return this;
        }
        /// <summary>
        /// 插件卸载处理
        /// </summary>
        /// <param name="plg"></param>
        protected override void PluginUnload(PluginObject plg)
        {
            base.PluginUnload(plg);
            string tkey = Assembly2Key(plg.TargetAssembly);
            PushConcurrentTask(() =>
            {
                AssemblyLoadContext asscontext;
                if (_assemblyContexts.TryGetValue(tkey, out asscontext))
                {
                    asscontext.Unload();
                    _assemblyContexts.Remove(tkey);
                }
            }, TimeSpan.FromMinutes(1));
        }

        private string Assembly2Key(Assembly assembly)
        {
            string guid = assembly.ManifestModule.ModuleVersionId.ToString();
            return assembly.GetHashCode() + guid;
        }
        /// <summary>
        /// 重写程序集加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override Assembly LoadAssembly(string path)
        {
            AssemblyLoadContext alc = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), _useUnload);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Assembly ass = alc.LoadFromStream(fs);
                _assemblyContexts.Add(Assembly2Key(ass), alc);
                return ass;
            }
        }

        public TANetCoreHttpApplication(IApplicationBuilder appbuilder)
        {
            _requestDelegate = appbuilder.Build();
            _appbuilder = appbuilder;
            UseParamMapping(new BodyParamMapping());
        }

        public HttpContext CreateContext(IFeatureCollection contextFeatures)
        {
            contextFeatures.Set<TANetCoreHttpApplication>(this);
            DefaultHttpContext df = new DefaultHttpContext(contextFeatures);
            df.RequestServices = this.ServiceProvider.GetService<IServiceProvider>();
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
