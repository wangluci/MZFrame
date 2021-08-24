using System;
using TemplateAction.Core;
using TemplateAction.Extension;
namespace TemplateAction.NetCore
{
    public class TANetServiceProvider : IServiceProvider
    {
        private ITAServices _services;
        public TANetServiceProvider(ITAServices services)
        {
            _services = services;
        }
        public object? GetService(Type serviceType)
        {
            return _services.GetService(serviceType);
        }
    }
}
