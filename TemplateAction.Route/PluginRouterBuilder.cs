using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    public class PluginRouterBuilder : AbstractRouterBuilder, IPluginRouterBuilder
    {
        public void AddRouter(string ns, string controller, string action, string template)
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add(TAUtility.NS_KEY, ns);
            defaults.Add(TAUtility.CONTROLLER_KEY, controller);
            defaults.Add(TAUtility.ACTION_KEY, action);
            _tmplist.Add(new Router(template, defaults));
        }
    }
}
