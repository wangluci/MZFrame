using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpHost
    {
        protected ServiceCollection _servicecollection;
        protected ILoggerFactory _logger;
        protected ApplicationLifetime _applifetime;
        protected IApplicationBuilder _appBuilder;
        protected TANetCoreHttpApplication _app;
        protected HostingEnvironment _hostingEnvironment;
        protected IConfiguration _config;

        private SocketTransportOptions _socketOptions;
        private KestrelServer _server;
        private bool _startedServer = false;
        private bool _stopped = false;
        private string _webroot = "Web";
        private string _workroot;
        private KestrelServerOptions _kestrelOptions;
        public const string WORK_PATH = "TA_WorkPath";
        /// <summary>
        /// 服务开始事件
        /// </summary>
        public const string SERVER_STARTED = "TA_Started";
        /// <summary>
        /// 服务中止中事件
        /// </summary>
        public const string SERVER_STOPPING = "TA_Stopping";
        /// <summary>
        /// 服务已中止事件
        /// </summary>
        public const string SERVER_STOPPED = "TA_Stopped";
        public TANetCoreHttpHost(IConfiguration config, ServiceCollection services)
        {
            _workroot = config[WORK_PATH];
            _config = config;
            _servicecollection = services;
            _servicecollection.AddLogging((logginbuilder) =>
            {
                logginbuilder.AddConfiguration(_config.GetSection("Logging")).AddConsole();
            });
            _hostingEnvironment = new HostingEnvironment();
            WebHostOptions hostoptions = new WebHostOptions(_config, Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty);
            string webroot = Path.Combine(_workroot, _webroot);
            if (!Directory.Exists(webroot))
            {
                Directory.CreateDirectory(webroot);
            }
            hostoptions.WebRoot = webroot;
            _hostingEnvironment.Initialize(_workroot, hostoptions);
            _servicecollection.AddSingleton<IHostingEnvironment>(_hostingEnvironment);


            ServiceProvider serviceprovider = _servicecollection.BuildServiceProvider();
            _logger = serviceprovider.GetRequiredService<ILoggerFactory>();
            _applifetime = new ApplicationLifetime(_logger.CreateLogger<ApplicationLifetime>());
            _applifetime.ApplicationStarted.Register(() =>
            {
                TemplateAction.Core.TAEventDispatcher.Instance.Dispatch(SERVER_STARTED, _app);
            });
            _applifetime.ApplicationStopping.Register(() =>
            {
                TemplateAction.Core.TAEventDispatcher.Instance.Dispatch(SERVER_STOPPING, _app);
            });
            _applifetime.ApplicationStopped.Register(() =>
            {
                TemplateAction.Core.TAEventDispatcher.Instance.Dispatch(SERVER_STOPPED, _app);
            });
            _kestrelOptions = new KestrelServerOptions();
            _kestrelOptions.ApplicationServices = serviceprovider;
            _kestrelOptions.Configure(_config.GetSection("Kestrel"));

            if (TemplateAction.Core.TAEventDispatcher.Instance.IsExistHandler<ListenOptions>())
            {
                _kestrelOptions.ConfigureEndpointDefaults(ac =>
                {
                    TemplateAction.Core.TAEventDispatcher.Instance.Dispatch(ac);
                });
            }
            _socketOptions = new SocketTransportOptions();
            _server = new KestrelServer(Options.Create(_kestrelOptions), new SocketTransportFactory(Options.Create(_socketOptions), _applifetime, _logger), _logger);
            _appBuilder = new ApplicationBuilderFactory(serviceprovider).CreateBuilder(_server.Features);
            _appBuilder.ApplicationServices = serviceprovider;
            TemplateAction.Core.TAEventDispatcher.Instance.Dispatch(_appBuilder);
        }


        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        private async Task StartAsync(CancellationToken token)
        {
            if (_startedServer) return;
            _app = new TANetCoreHttpApplication(_appBuilder);
            _app.Init(Directory.GetCurrentDirectory());
            _appBuilder.ApplicationServices = _servicecollection.AddSingleton<TANetCoreHttpApplication>(_app).BuildServiceProvider();
            await _server.StartAsync(_app, token).ConfigureAwait(false);
            _startedServer = true;
            _applifetime.NotifyStarted();
        }
        private async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_stopped) return;
            _stopped = true;
            if (_startedServer)
            {
                await _server.StopAsync(cancellationToken).ConfigureAwait(false);
            }
            _applifetime?.NotifyStopped();
            if (_app != null)
            {
                ServiceDescriptor serviceDescriptor = _servicecollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TANetCoreHttpApplication));
                if (serviceDescriptor != null) _servicecollection.Remove(serviceDescriptor);
                _app.Dispose();
                _app = null;
            }
        }
        private async Task RunAsync(CancellationToken token = default)
        {
            // Wait for token shutdown if it can be canceled
            if (token.CanBeCanceled)
            {
                await RunAsyncNeedToken(token);
                return;
            }

            //如果token不能被取消，则使用 Ctrl+C和SIGTERM 关闭应用程序
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                using (TANetCoreHttpShutdownTrigger lifetime = new TANetCoreHttpShutdownTrigger(cts, done))
                {
                    try
                    {
                        await RunAsyncNeedToken(cts.Token);
                        lifetime.SetExitedGracefully();
                    }
                    finally
                    {
                        done.Set();
                    }
                }
            }
        }
        private async Task RunAsyncNeedToken(CancellationToken token)
        {
            try
            {
                await StartAsync(token);
                await WaitForTokenShutdownAsync(token);
            }
            finally
            {
                await StopAsync();
            }
        }

        private async Task WaitForTokenShutdownAsync(CancellationToken token)
        {
            TaskCompletionSource<object> waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register((obj) =>
            {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForStop);
            await waitForStop.Task;
        }
    }
}
