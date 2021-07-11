using System;

namespace TemplateAction.Core
{
    public interface IFilterMiddleware
    {
        object Excute(TARequestHandle request, IFilterMiddleware next);
    }
}
