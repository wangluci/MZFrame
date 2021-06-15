using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public interface IRequestHandle: ITemplatePath
    {
        ITAObjectCollection ExtParams { get; }
        void AddGlobal(string key, object value);
        T Global<T>(string key, T def);
        /// <summary>
        /// 判断是否定义了指定全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsDefine(string key);
        ITAContext Context { get; }
        ITemplateContext TemplateContext { get; }
        object Excute(Type controllerType, ActionNode action);
    }
}
