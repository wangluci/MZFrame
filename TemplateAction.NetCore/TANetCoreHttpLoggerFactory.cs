using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpLoggerFactory: ITALoggerFactory
    {
        private ILoggerFactory _factory;
        public TANetCoreHttpLoggerFactory(IServiceProvider serviceProvider)
        {
            _factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        }
        public ITALogger CreateLogger(string categoryName)
        {
            return new TANetCoreHttpLogger(_factory.CreateLogger(categoryName));
        }
    }
}
