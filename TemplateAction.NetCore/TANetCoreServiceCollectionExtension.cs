using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using TemplateAction.Core;
namespace TemplateAction.NetCore
{
    public static class TANetCoreServiceCollectionExtension
    {
        public static IServiceCollection AddOptions(this IServiceCollection collection)
        {
            collection.TryAddSingleton(typeof(IOptions<>), typeof(OptionsManager<>));
            collection.TryAddScope(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>));
            collection.TryAddSingleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>));
            collection.TryAddTransient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>));
            collection.TryAddSingleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>));
            return collection;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name, configureOptions));
            return services;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
        {
            return services.Configure(string.Empty, configureOptions);
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name, config));
            services.AddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, config));
            return services;
        }
    }
}
