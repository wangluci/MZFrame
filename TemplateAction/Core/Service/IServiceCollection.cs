
using System;
namespace TemplateAction.Core
{
    public interface IServiceCollection
    {
        IServiceDescriptorEnumerable this[string key] { get; }
        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="key"></param>
        /// <param name="des"></param>
        void Add(string key, ServiceDescriptor des);
        /// <summary>
        /// 不存在才注入服务
        /// </summary>
        /// <param name="key"></param>
        /// <param name="des"></param>
        /// <returns></returns>

        bool TryAdd(string key, ServiceDescriptor des);
    }
}
