using System;

namespace TemplateAction.Core
{
    public interface IFilterMiddleware
    {
        object Excute(TAAction request, FilterMiddlewareNode next);
    }
}
