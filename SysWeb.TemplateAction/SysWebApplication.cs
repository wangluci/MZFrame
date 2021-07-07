using System;
using System.Web;
using System.Web.Hosting;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    /// <summary>
    /// 完全懒汉单例
    /// </summary>
    public class SysWebApplication
    {
        private volatile static TAApplication _app = null;
        private static readonly object lockobj = new object();
        public static TAApplication GetInstance(HttpContext context)
        {
            if (_app == null)
            {
                lock (lockobj)
                {
                    if (_app == null)
                    {
                        TAApplication tmpapp = new TAApplication().Init(context.Server.MapPath("/"));
                        if (tmpapp.ReadAssetsFromPlugin)
                        {
                            HostingEnvironment.RegisterVirtualPathProvider(new AssetsMapPathProvider(tmpapp));
                        }
                        _app = tmpapp;
                    }
                }
            }
            return _app;
        }
    }
}
