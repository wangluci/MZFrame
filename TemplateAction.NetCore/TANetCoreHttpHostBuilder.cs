﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHostBuilder
    {
        private DefaultMultiHandler<IApplicationBuilder> _appBuilderEvents;
        private DefaultMultiHandler<ListenOptions> _middlewareEvents;
        private IConfiguration _config;
        private Microsoft.Extensions.DependencyInjection.ServiceCollection _services;

        private Action<IApplicationBuilder> _configAC;
        private Action<ListenOptions> _middleAC;
        private Action<IConfiguration, ILoggingBuilder> _loggingAC;
        private Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> _serviceAC;
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
        /// <summary>
        /// 创建默认配置的TANetCoreHttpHostBuilder
        /// </summary>
        /// <returns></returns>
        public static TANetCoreHttpHostBuilder CreateDefaultHostBuilder()
        {
            return new TANetCoreHttpHostBuilder().Configure((IApplicationBuilder builder) =>
            {
                builder.UseStaticFiles();
                builder.UseTAMvc();
            }).ConfigureLogging((config, logginbuilder) =>
            {
                logginbuilder.AddConfiguration(config.GetSection("Logging")).AddConsole();
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
    

        /// <summary>
        /// 配置Http中间件
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder Configure(Action<IApplicationBuilder> ac)
        {
            _configAC = ac;
            return this;
        }
        /// <summary>
        /// 配置中间件
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder ConfigureMiddleware(Action<ListenOptions> ac)
        {
            _middleAC = ac;
            return this;
        }
        /// <summary>
        /// 配置日志服务
        /// </summary>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder ConfigureLogging(Action<IConfiguration, ILoggingBuilder> ac)
        {
            _loggingAC = ac;
            return this;
        }
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public TANetCoreHttpHostBuilder ConfigureServices(Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> ac)
        {
            _serviceAC = ac;
            return this;
        }
        public TANetCoreHttpHost Build()
        {
            //初始化配置
            if (_configAC != null)
            {
                _appBuilderEvents.Register(_configAC);
                TAEventDispatcher.Instance.Register<IApplicationBuilder>(_appBuilderEvents);
            }
            if (_middleAC != null)
            {
                _middlewareEvents.Register(_middleAC);
                TAEventDispatcher.Instance.Register<ListenOptions>(_middlewareEvents);
            }
            if (_loggingAC != null)
            {
                _services.AddLogging(loggingbuilder => _loggingAC(_config, loggingbuilder));
            }
            _serviceAC?.Invoke(_services);

            return new TANetCoreHttpHost(_config, _services);
        }
    }
}
