using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    public static class PluginObjectExtension
    {

        public static Dictionary<string, ControllerNode> GetControllerList(this PluginObject obj)
        {
            return obj.Data.Get<Dictionary<string, ControllerNode>>();
        }
        public static bool ContainController(this PluginObject obj, string key)
        {
            return obj.Data.Get<Dictionary<string, ControllerNode>>().ContainsKey(key);
        }
        public static ControllerNode GetControllerNodeByKey(this PluginObject obj, string controller)
        {
            ControllerNode rtVal = null;

            if (obj.Data.Get<Dictionary<string, ControllerNode>>().TryGetValue(controller.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public static ActionNode GetMethodByKey(this PluginObject obj, string controller, string action)
        {
            ControllerNode rtVal = null;
            if (obj.Data.Get<Dictionary<string, ControllerNode>>().TryGetValue(controller.ToLower(), out rtVal))
            {
                ActionNode an = rtVal.GetChildNode(action) as ActionNode;
                if (an != null)
                {
                    return an;
                }
            }
            return null;
        }

        public static IDictionary<string, object> Route(this PluginObject obj, ITAContext context)
        {
            IRouterCollection rc = obj.Data.Get<IRouterCollection>();
            if (rc == null)
            {
                return null;
            }
            return rc.Route(context);
        }
    }
}
