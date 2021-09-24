using System;

namespace TemplateAction.Core
{
    public interface IFilterMiddleware
    {
        object Excute(TAAction ac, FilterMiddlewareNode next);
    }
}
