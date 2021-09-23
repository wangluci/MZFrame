﻿using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Common.Redis;
using Microsoft.Extensions.Options;

namespace AuthService
{
    public class PluginConfig : IPluginConfig
    {
        private IServiceCollection _services;
        public void Configure(IServiceCollection services)
        {
            _services = services;
            services.AddSingleton<AuthBLL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateBLL(typeof(AuthBLL), arguments);
            });
            services.AddSingleton<AuthDAL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(AuthDAL), arguments);
            });
            services.AddSingleton<PermissionDAL>((object[] arguments, ITAServices provider) =>
            {
                return MyAccess.Aop.InterceptFactory.CreateDAL(typeof(PermissionDAL), arguments);
            });
            services.AddSingleton<AuthRedisHelper>();
        }
        public void Loaded(ITAApplication app, IEventRegister register)
        {
            IConfiguration config = app.ServiceProvider.GetService<IConfiguration>();
            _services.Configure<AuthOption>(config.GetSection("AuthService"));
        }
        public void Unload() { }
    }
}
