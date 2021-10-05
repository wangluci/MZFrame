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
        private volatile static TASiteApplication _app = null;
        private static readonly object lockobj = new object();
        public static TASiteApplication GetInstance(HttpContext context)
        {
            if (_app == null)
            {
                lock (lockobj)
                {
                    if (_app == null)
                    {
                        _app = new TASiteApplication().Init(context.Server.MapPath("~"), HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly);
                    }
                }
            }
            return _app;
        }
    }
}
