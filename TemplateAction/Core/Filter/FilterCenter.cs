using System;
namespace TemplateAction.Core
{
    public class FilterCenter
    {
        /// <summary>
        /// Action拦截器中间件列表
        /// </summary>
        private FilterMiddlewareNode _first;
        private FilterMiddlewareNode _lastMvc;
        public FilterCenter()
        {
            //添加Mvc中间件
            FilterMiddlewareNode node = new FilterMiddlewareNode(new MvcMiddleware());
            node.Pre = null;
            node.Next = null;
            _first = node;
            _lastMvc = node;
        }
        public void Clear()
        {
            while (_first != _lastMvc)
            {
                FilterMiddlewareNode nextnode = _first.Next;
                nextnode.Pre = null;
                _first = nextnode;
            }
        }
        public object Excute(TAAction request)
        {
            return _first.Excute(request);
        }
        /// <summary>
        /// 添加在最后， 也就是mvc前面
        /// </summary>
        /// <param name="filter"></param>
        public void Add(IFilterMiddleware filter)
        {
            FilterMiddlewareNode node = new FilterMiddlewareNode(filter);
            node.Next = _lastMvc;
            if (_lastMvc.Pre == null)
            {
                node.Pre = null;
                _first = node;
            }
            else
            {
                node.Pre = _lastMvc.Pre;
                node.Pre.Next = node;
            }
            _lastMvc.Pre = node;
        }
        /// <summary>
        /// 添加在第一个位置，也就是mvc之前
        /// </summary>
        /// <param name="filter"></param>
        public void AddFirst(IFilterMiddleware filter)
        {
            FilterMiddlewareNode node = new FilterMiddlewareNode(filter);
            _first.Pre = node;
            node.Next = _first;
            node.Pre = null;
            _first = node;
        }
    }
}
