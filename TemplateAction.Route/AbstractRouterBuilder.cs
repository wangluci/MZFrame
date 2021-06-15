using System;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class AbstractRouterBuilder : IRouterCollectionBuilder
    {
        protected IRouterCollection _tmplist = new RouterCollection();
        public IRouterCollection Build()
        {
            return _tmplist;
        }
    }
}
