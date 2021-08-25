
using System;

namespace TemplateAction.Core
{
    public interface IServiceCollection
    {
        ServiceDescriptor this[string key] { get; }
        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="key"></param>
        /// <param name="des"></param>
        void Add(string key, ServiceDescriptor des);
    }
}
