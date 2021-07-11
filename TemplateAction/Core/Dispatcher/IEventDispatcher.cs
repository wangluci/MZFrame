using System;
namespace TemplateAction.Core
{
    internal interface IEventDispatcher : IDispatcher, IEventRegister
    {
        void DispathLoadAfter<T>(T app) where T : TAApplication;
    }
}
