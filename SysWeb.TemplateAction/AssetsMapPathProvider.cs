using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class AssetsMapPathProvider : VirtualPathProvider
    {
        private TAApplication _app;
        public AssetsMapPathProvider(TAApplication app)
        {
            _app = app;
        }
        /// <summary>
        /// 如果我们能找到这个虚拟路径，返回true
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override bool FileExists(string virtualPath)
        {
            if (base.FileExists(virtualPath)) return true;
            if (virtualPath.StartsWith("~"))
            {
                virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);
            }
            if (_app.ExistPluginAssets(virtualPath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 使用自定义VirtualFile类加载程序集资源
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            if (base.FileExists(virtualPath)) return base.GetFile(virtualPath);
            if (virtualPath.StartsWith("~"))
            {
                virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);
            }
            byte[] data = _app.FindPluginAssets(virtualPath);
            if (data != null)
            {
                return new AssetsFile(virtualPath, data);
            }
            return null;
        }

        /// <summary>
        /// 当应用程序使用虚拟文件路径时返回null
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="virtualPathDependencies"></param>
        /// <param name="utcStart"></param>
        /// <returns></returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (base.FileExists(virtualPath))
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
            else
            {
                return null;
            }
        }

    }
}
