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
            collection.AddSingleton(typeof(IOptions<>), typeof(OptionsManager<>));
            collection.AddScope(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>));
            collection.AddSingleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>));
            collection.AddTransient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>));
            collection.AddSingleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>));
            return collection;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>((object[] arguments) =>
            {
                return new ConfigureNamedOptions<TOptions>(name, configureOptions);
            });
            return services;
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
        {
            return services.Configure(string.Empty, configureOptions);
        }
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>((object[] arguments) =>
            {
                return new ConfigurationChangeTokenSource<TOptions>(name, config);
            });
            services.AddSingleton<IConfigureOptions<TOptions>>((object[] arguments) =>
            {
                return new NamedConfigureFromConfigurationOptions<TOptions>(name, config);
            });
            return services;
        }
    }
}
