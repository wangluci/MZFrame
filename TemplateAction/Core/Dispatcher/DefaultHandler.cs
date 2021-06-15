using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 单个事件处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultHandler<T> : ITAEventHandler<T> where T : class
    {
        private Action<T> _configac;
        public DefaultHandler(Action<T> ac)
        {
            _configac = ac;
        }
        public void OnEvent(T evt)
        {
            _configac(evt);
        }
    }
}
