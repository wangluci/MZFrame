using System;

namespace TemplateAction.Core
{
    public interface IEventRegister
    {
        void Register<T, X>(string key, X handler) where T : class where X : ITAEventHandler<T>;
        void RegisterLoadAfter<T>(Action<T> ac) where T: TAApplication;
        /// <summary>
        /// 通过监听器监听事件
        /// </summary>
        /// <param name="interfacename"></param>
        /// <param name="listener"></param>
        void AddListener(string interfacename, object listener);
        void AddListener(object listener);

    }
}
