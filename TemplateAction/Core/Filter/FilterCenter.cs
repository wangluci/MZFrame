using System;
namespace TemplateAction.Core
{
    public class FilterCenter
    {
        /// <summary>
        /// Action拦截器中间件列表
        /// </summary>
        private FilterMiddlewareNode _first;
        public FilterCenter()
        {   
            //添加Mvc中间件
            AddFirst(new MvcMiddleware());
        }
        public void Clear()
        {
            while (_first.GetType() != typeof(MvcMiddleware))
            {
                FilterMiddlewareNode nextnode = _first.Next;
                nextnode.Pre = null;
                _first = nextnode;
            }
        }
        public void AddFirst(IFilterMiddleware filter)
        {
            FilterMiddlewareNode node = new FilterMiddlewareNode(filter);
            if (_first == null)
            {
                node.Pre = null;
                node.Next = null;
            }
            else
            {
                _first.Pre = node;
                node.Next = _first;
                node.Pre = null;
            }
            _first = node;
        }
    }
}
