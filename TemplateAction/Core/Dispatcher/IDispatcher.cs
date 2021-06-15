using System;

namespace TemplateAction.Core
{
    public interface IDispatcher
    {
        void Dispatch<T>(string key, T evt) where T : class;
        bool IsExistDispatcher(string key);
    }
}
