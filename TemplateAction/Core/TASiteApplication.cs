using System;
using System.IO;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// web应用程序
    /// </summary>
    public class TASiteApplication : TAApplication
    {
        /// <summary>
        /// 是否从插件读取资源
        /// </summary>
        private bool _readAssetsFromPlugin;
        public bool ReadAssetsFromPlugin
        {
            get { return _readAssetsFromPlugin; }
        }
        private FilterCenter _filterCenter;
        internal FilterCenter Filters
        {
            get { return _filterCenter; }
        }
        public TASiteApplication()
        {
            _filterCenter = new FilterCenter();
            _readAssetsFromPlugin = true;
        }
        /// <summary>
        /// 创建实例并自动创建引用的参数实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        internal object CreateInstance(Type serviceType)
        {
            return _plugins.CreateServiceInstance(serviceType);
        }
        /// <summary>
        /// 使用指定中间件，需要先AddSingle注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TASiteApplication UseFilterMiddleware<T>() where T : class, IFilterMiddleware
        {
            UseFilterMiddleware(typeof(T).FullName);
            return this;
        }
        /// <summary>
        /// 使用指定中间件，需要先AddSingle注册
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TASiteApplication UseFilterMiddleware(string key)
        {
            _filterCenter.AddFirst(new PluginMiddleware(key));
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TASiteApplication ClearFilter()
        {
            _filterCenter.Clear();
            return this;
        }
        /// <summary>
        /// 开始路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TARequestHandleBuilder Route(ITAContext context)
        {
            IDictionary<string, object> dict = _plugins.Route(context);
            if (!Equals(dict, null))
            {
                string tns = string.Empty;
                string tcontroller = string.Empty;
                string taction = context.Request.HttpMethod;
                TAObjectCollection exparams = new TAObjectCollection();
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case TAUtility.NS_KEY:
                            tns = kvp.Value as string;
                            break;
                        case TAUtility.CONTROLLER_KEY:
                            tcontroller = kvp.Value as string;
                            break;
                        case TAUtility.ACTION_KEY:
                            taction = kvp.Value as string;
                            break;
                        default:
                            exparams.Add(kvp.Key, kvp.Value);
                            break;
                    }
                }
                //清除线程服务
                _plugins.ClearThreadService();
                return CreateTARequestHandleBuilder(context, tns, tcontroller, taction, exparams);
            }
            return null;
        }
        public TARequestHandleBuilder CreateTARequestHandleBuilder(ITAContext context, string ns, string controller, string action, ITAObjectCollection ext = null)
        {
            return new TARequestHandleBuilder(_plugins, context, ns, controller, action, ext);
        }
        /// <summary>
        /// 使用指定路由创建工厂并加载全局路由
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public TASiteApplication UseRouterBuilder(IRouterBuilder builder)
        {
            _plugins.UseRouter(builder);
            return this;
        }
        public List<AnnotationInfo> FindActinAnnotationList()
        {
            return _plugins.ViewAnnList();
        }
        /// <summary>
        /// 调用者会从各个插件中复制资源文件
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public TASiteApplication UsePluginAssets(bool enable)
        {
            _readAssetsFromPlugin = enable;
            return this;
        }
        public TASiteApplication UsePluginPath(string path)
        {
            _pluginPath = path;
            return this;
        }
        public TASiteApplication Init(string rootpath)
        {
            TAInit(rootpath);
            return this;
        }

        public bool ControllerExist(string controller)
        {
            return _plugins.ContainController(controller);
        }
        protected override void AfterPluginChanged(PluginObject plg)
        {
            if (_readAssetsFromPlugin)
            {
                UpdateDocument(plg);
            }
            base.AfterPluginChanged(plg);
        }
        protected override void BeforeInit()
        {
            //激活配置文件
            TAEventDispatcher.Instance.DispathLoadBefore(this);
        }
        protected override void AfterInit(List<PluginObject> plglist)
        {
            foreach (PluginObject plg in plglist)
            {
                UpdateDocument(plg);
            }
            base.AfterInit(plglist);
        }
        /// <summary>
        /// 更新模块资源
        /// </summary>
        private void UpdateDocument(PluginObject plg)
        {
            System.Reflection.Assembly assem = plg.TargetAssembly;
            foreach (string item in assem.GetManifestResourceNames())
            {
                Stream inputStream = assem.GetManifestResourceStream(item);
                string tmpstr = string.Empty;
                if (item.EndsWith(TAUtility.FILE_EXT, StringComparison.OrdinalIgnoreCase))
                {
                    tmpstr = item.Substring(0, item.Length - TAUtility.FILE_EXT.Length);
                    string[] patharr = tmpstr.Split(new char[] { '.' });
                    if (patharr.Length == 0) continue;
                    tmpstr = string.Empty;
                    for (int i = 0; i < patharr.Length; i++)
                    {
                        tmpstr = tmpstr + Path.DirectorySeparatorChar + patharr[i];
                    }
                    tmpstr = tmpstr + TAUtility.FILE_EXT;
                    if (tmpstr[0] == Path.DirectorySeparatorChar)
                    {
                        tmpstr = tmpstr.Substring(1);
                    }

                    using (StreamReader sr = new StreamReader(inputStream))
                    {
                        TemplateApp.Instance.AddViewPage(Path.Combine(_rootPath, tmpstr), sr.ReadToEnd(), plg.CacheDependency);
                    }

                }


            }
        }
    }
}
