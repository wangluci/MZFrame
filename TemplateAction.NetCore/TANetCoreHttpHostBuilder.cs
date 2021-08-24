using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using TemplateAction.Core;
using TemplateAction.Route;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHostBuilder
    {
        private DefaultMultiHandler<IApplicationBuilder> _appBuilderEvents;
        private DefaultMultiHandler<ListenOptions> _middlewareEvents;
        private IConfiguration _config;
        private Microsoft.Extensions.DependencyInjection.ServiceCollection _services;
        private Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> _servicesac;
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public IConfiguration Config { get { return _config; } }
        public TANetCoreHttpHostBuilder()
        {
            _services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            _appBuilderEvents = new DefaultMultiHandler<IApplicationBuilder>();
            _middlewareEvents = new DefaultMultiHandler<ListenOptions>();
            string rootpath = Directory.GetCurrentDirectory();
            _config = new ConfigurationBuilder()
   .SetBasePath(rootpath)//设置基础路径
   .AddJsonFile("appsettings.json", true, true)//加载配置文件
   .Build();
            _config[TANetCoreHttpHost.WORK_PATH] = rootpath;
        }
        public static TANetCoreHttpHostBuilder CreateDefaultHostBuilder()
        {
            TANetCoreHttpHostBuilder destHostBuilder = new TANetCoreHttpHostBuilder();
            return destHostBuilder.Configure((IApplicationBuilder builder) =>
            {
                builder.UseStaticFiles();
                builder.UseTAMvc(app =>
                {
                    app.Services.AddSingleton<IConfiguration>((object[] arguments) =>
                    {
                        return destHostBuilder._config;
                    });
                    //映射服务
                    app.Services.AddSingleton<IServiceProvider, TANetServiceProvider>();
                    //添加日志
                    app.Services.AddSingleton<ITALoggerFactory, TANetCoreHttpLoggerFactory>((object[] arguments) =>
                    {
                        return new TANetCoreHttpLoggerFactory(builder.ApplicationServices);
                    });

                    //设置路由
                    string defns = null;
                    Assembly ass = Assembly.GetEntryAssembly();
                    if (ass != null)
                    {
                        defns = ass.GetName().Name;
                    }
                    RouterBuilder rtbuilder = new RouterBuilder();
                    rtbuilder.UsePlugin();
                    if (defns != null)
                    {
                        rtbuilder.UseDefault(defns);
                    }
                    app.UseRouterBuilder(rtbuilder);

                });
            });
        }
        public TANetCoreHttpHostBuilder UseHttpUrl(string url)
        {
            _config["Kestrel:EndPoints:Http:Url"] = url;
            return this;
        }
        public TANetCoreHttpHostBuilder UseHttpsUrl(string url, string path, string password)
        {
            _config["Kestrel:EndPoints:Https:Url"] = url;
            _config["Kestrel:EndPoints:Https:Certificate:Path"] = path;
            _config["Kestrel:EndPoints:Https:Certificate:Password"] = password;
            return this;
        }
        public TANetCoreHttpHostBuilder UseMiddleware(Action<ListenOptions> ac)
        {
            _middlewareEvents.Register(ac);
            TAEventDispatcher.Instance.Register<ListenOptions>(_middlewareEvents);
            return this;
        }
        /// <summary>
        /// 配置Http中间件
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder Configure(Action<IApplicationBuilder> ac)
        {
            _appBuilderEvents.Register(ac);
            TAEventDispatcher.Instance.Register<IApplicationBuilder>(_appBuilderEvents);
            return this;
        }
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder ConfigureServices(Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> ac)
        {
            _servicesac = ac;
            return this;
        }
        public TANetCoreHttpHost Build()
        {
            if (_servicesac != null)
            {
                _servicesac(_services);
            }
            return new TANetCoreHttpHost(_config, _services);
        }
    }
}
