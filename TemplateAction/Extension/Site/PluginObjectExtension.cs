using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Extension.Site
{
    public static class PluginObjectExtension
    {

        public static Dictionary<string, ControllerNode> GetControllerList(this PluginObject obj)
        {
            return ((SiteExtentionData)obj.Data).ControllerList;
        }
        public static bool ContainController(this PluginObject obj, string key)
        {
            return ((SiteExtentionData)obj.Data).ControllerList.ContainsKey(key);
        }
        public static ControllerNode GetControllerNodeByKey(this PluginObject obj, string controller)
        {
            ControllerNode rtVal = null;

            if (((SiteExtentionData)obj.Data).ControllerList.TryGetValue(controller.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public static ActionNode GetMethodByKey(this PluginObject obj, string controller, string action)
        {
            ControllerNode rtVal = null;
            if (((SiteExtentionData)obj.Data).ControllerList.TryGetValue(controller.ToLower(), out rtVal))
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
            SiteExtentionData sitedata = (SiteExtentionData)obj.Data;
            if (sitedata.RouterCollection == null)
            {
                return null;
            }
            return sitedata.RouterCollection.Route(context);
        }
    }
}
