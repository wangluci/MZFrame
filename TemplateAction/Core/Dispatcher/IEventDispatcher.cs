using System;
namespace TemplateAction.Core
{
    internal interface IEventDispatcher : IDispatcher, IEventRegister
    {
        void DispathLoadAfter(TAApplication app);
    }
}
