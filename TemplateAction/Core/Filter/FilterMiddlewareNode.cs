using System;

namespace TemplateAction.Core
{
    public class FilterMiddlewareNode
    {
        private FilterMiddlewareNode _pre;
        public FilterMiddlewareNode Pre
        {
            get { return _pre; }
            internal set { _pre = value; }
        }
        private FilterMiddlewareNode _next;
        public FilterMiddlewareNode Next
        {
            get { return _next; }
            internal set { _next = value; }
        }
        private IFilterMiddleware _filter;
        public FilterMiddlewareNode(IFilterMiddleware filter)
        {
            _filter = filter;
        }

        public object Excute(TAAction ac)
        {
            return _filter.Excute(ac, _next);
        }
    }
}
