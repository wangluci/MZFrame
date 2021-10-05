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
        public static List<DescribeInfo> FindAllDescribe(this PluginCollection collection)
        {
            PluginObject[] tarr = collection.GetAllPlugin();

            List<DescribeInfo> rtlist = new List<DescribeInfo>();
            foreach (PluginObject plg in tarr)
            {
                Dictionary<string, ControllerNode> ml = plg.GetControllerList();
                foreach (KeyValuePair<string, ControllerNode> kpcn in ml)
                {
                    if (!string.IsNullOrEmpty(kpcn.Value.Descript))
                    {
                        DescribeInfo ai = new DescribeInfo();
                        ai.Name = kpcn.Value.Descript;
                        ai.Sort = kpcn.Value.Sort;
                        ai.Code = string.Format("/{0}/{1}/", plg.Name, kpcn.Value.Key);
                        ai.ParentCode = string.Format("/{0}/", plg.Name);
                        rtlist.Add(ai);
                        foreach (KeyValuePair<string, Node> kpn in kpcn.Value.Childrens)
                        {
                            ActionNode an = kpn.Value as ActionNode;
                            if (an == null) continue;
                            if (!string.IsNullOrEmpty(kpn.Value.Descript))
                            {
                                DescribeInfo aii = new DescribeInfo();
                                aii.Name = kpn.Value.Descript;
                                aii.Sort = kpn.Value.Sort;
                                aii.Code = string.Format("/{0}/{1}/{2}",plg.Name, kpcn.Value.Key, kpn.Value.Key);
                                aii.ParentCode = ai.Code;
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
        public static bool ExistController(this PluginCollection collection, string key)
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
