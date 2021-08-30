using System;
using TemplateAction.Core;
using System.Collections.Generic;

namespace TemplateAction.Extension.Site
{
    public static class PluginCollectionExtension
    {
        public static PluginCollection UseRouter(this PluginCollection collection, IRouterBuilder builder)
        {
            SitePluginCollectionExtData sitefactory = (SitePluginCollectionExtData)collection.ExtentionData;
            sitefactory.RouterBuilder = builder;
            sitefactory.RouterCollection = builder.Build();
            return collection;
        }
        /// <summary>
        /// 路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<string, object> Route(this PluginCollection collection, ITAContext context)
        {
            SitePluginCollectionExtData sitefactory = (SitePluginCollectionExtData)collection.ExtentionData;
            if (sitefactory.RouterCollection == null)
            {
                return null;
            }
            //先路由全局
            IDictionary<string, object> rt = sitefactory.RouterCollection.Route(context);
            if (rt != null)
            {
                return rt;
            }
            //再路由插件
            PluginObject[] tarr = collection.GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                rt = plg.Route(context);
                if (rt != null)
                {
                    return rt;
                }
            }
            return rt;
        }
        /// <summary>
        /// 获取控制器和动作的描述信息
        /// </summary>
        /// <returns></returns>
        public static List<AnnotationInfo> ViewAnnList(this PluginCollection collection)
        {
            PluginObject[] tarr = collection.GetAllPlugin();

            List<AnnotationInfo> rtlist = new List<AnnotationInfo>();
            foreach (PluginObject plg in tarr)
            {
                Dictionary<string, ControllerNode> ml = plg.GetControllerList();
                foreach (KeyValuePair<string, ControllerNode> kpcn in ml)
                {
                    if (!string.IsNullOrEmpty(kpcn.Value.Descript))
                    {
                        AnnotationInfo ai = new AnnotationInfo();
                        ai.Name = kpcn.Value.Descript;
                        ai.Code = string.Format("/{0}/{1}", plg.Name, kpcn.Key);
                        ai.ParentCode = string.Empty;
                        rtlist.Add(ai);
                        foreach (KeyValuePair<string, Node> kpn in kpcn.Value.Childrens)
                        {
                            ActionNode an = kpn.Value as ActionNode;
                            if (an == null) continue;
                            if (!string.IsNullOrEmpty(kpn.Value.Descript))
                            {
                                AnnotationInfo aii = new AnnotationInfo();
                                aii.Name = kpn.Value.Descript;
                                aii.Code = string.Format("{0}/{1}", ai.Code, kpn.Key);
                                aii.ParentCode = kpcn.Key;
                                rtlist.Add(aii);
                            }
                        }
                    }
                }
            }
            return rtlist;

        }



        /// <summary>
        /// 获取Controller节点
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static ControllerNode GetControllerByKeyInPlugin(this PluginCollection collection, string ns, string controller)
        {
            PluginObject pobj = collection.GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetControllerNodeByKey(controller);
            }
            return null;
        }

        /// <summary>
        /// 获取Action节点
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionNode GetMethodByKeyInPlugin(this PluginCollection collection, string ns, string controller, string action)
        {
            PluginObject pobj = collection.GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetMethodByKey(controller, action);
            }
            return null;
        }
        public static bool ContainController(this PluginCollection collection, string key)
        {
            PluginObject[] tarr = collection.GetAllPlugin();
            foreach (PluginObject plg in tarr)
            {
                if (plg.ContainController(key))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
