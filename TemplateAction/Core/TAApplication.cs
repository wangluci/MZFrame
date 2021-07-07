using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TemplateAction.Common;
using System.Collections.Concurrent;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 加载所有的插件
    /// </summary>
    public class TAApplication : IDisposable
    {
        private bool _disposed = false;
        private int _loaded = 0;
        /// <summary>
        /// 插件集
        /// </summary>
        private PluginCollection _plugins;
        private LinkedListNode<IDispatcher> _pluginsnode;
        //监控插件更改
        private FileSystemWatcher _watcher;
        private Timer _timer = null;
        private string _rootPath;
        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPath
        {
            get { return _rootPath; }
        }
        /// <summary>
        /// 插件目录
        /// </summary>
        private string _pluginPath;
        public IServiceCollection Services
        {
            get { return _plugins.Services; }
        }
        public ITAServices ServiceProvider
        {
            get { return _plugins; }
        }
        /// <summary>
        /// 是否从插件读取资源
        /// </summary>
        private bool _readAssetsFromPlugin;
        public bool ReadAssetsFromPlugin
        {
            get { return _readAssetsFromPlugin; }
        }
        ~TAApplication()
        {
            Dispose(false);
        }
        public TAApplication()
        {
            _readAssetsFromPlugin = true;
            _plugins = new PluginCollection();
            _pluginsnode = TAEventDispatcher.Instance.AddScope(_plugins);
        }

        /// <summary>
        /// 显示释放对象资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            TAEventDispatcher.Instance.RemoveScope(_pluginsnode);
            _watcher.Dispose();
            _timer.Dispose();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
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
        /// 使用指定路由创建工厂并加载全局路由
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal TAApplication SetRouterBuilder(IRouterBuilder builder)
        {
            _plugins.UseRouter(builder);
            return this;
        }
        /// <summary>
        /// 开始路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal TARequestHandleBuilder Route(ITAContext context)
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
        internal TARequestHandleBuilder CreateTARequestHandleBuilder(ITAContext context, string ns, string controller, string action, ITAObjectCollection ext = null)
        {
            ns = ns.ToLower();
            controller = controller.ToLower();
            action = action.ToLower();
            Type controllerType = _plugins.GetControllerByKeyInPlugin(ns, controller);
            ActionNode node = _plugins.GetMethodByKeyInPlugin(controller, action, ns);
            return new TARequestHandleBuilder(context, ns, controller, action, ext, controllerType, node);
        }
        internal List<AnnotationInfo> ViewAnnList()
        {
            return _plugins.ViewAnnList();
        }
        internal ControllerNode FindControllerNodeByKey(string ns, string controller)
        {
            return _plugins.GetControllerNodeByKeyInPlugin(ns, controller);
        }
        internal bool ContainController(string controller)
        {
            return _plugins.ContainController(controller);
        }
        public TAApplication UsePluginPath(string path)
        {
            _pluginPath = path;
            return this;
        }
        /// <summary>
        /// 调用者会从各个插件中复制资源文件
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public TAApplication UsePluginAssets(bool enable)
        {
            _readAssetsFromPlugin = enable;
            return this;
        }

        /// <summary>
        /// 获取指定插件的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ns"></param>
        /// <returns></returns>
        public T FindConfig<T>(string ns) where T : IPluginConfig
        {
            PluginObject obj = _plugins.GetPlugin(ns.ToLower());
            if (obj != null)
            {
                return (T)obj.Config;
            }
            return default(T);
        }

        /// <summary>
        /// 判断是否包含指定名称的插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool PluginExist(string ns)
        {
            return _plugins.ContainPlugin(ns);
        }
        /// <summary>
        /// 更新模块资源
        /// </summary>
        private void UpdateAssetsAndDocument(PluginObject plg)
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


        /// <summary>
        /// 初始化并加载目录下的插件
        /// </summary>
        /// <param name="rootpath"></param>
        /// <returns></returns>
        public TAApplication Init(string rootpath)
        {
            if (rootpath.Length > 0 && rootpath[rootpath.Length - 1] == Path.DirectorySeparatorChar)
            {
                rootpath = rootpath.Substring(0, rootpath.Length - 1);
            }
            if (_loaded == 1)
            {
                return this;
            }
            int orists = Interlocked.CompareExchange(ref _loaded, 1, 0);
            if (orists != 0)
            {
                return this;
            }
            //初始化根目录
            _rootPath = rootpath;
            //默认插件路径
            _pluginPath = Path.Combine(_rootPath, "Plugin");
            //激活配置文件
            TAEventDispatcher.Instance.DispathLoadBefore(this);
            //从应用程序域的程序集中初始化插件集
            _plugins.InitFromEntryAssembly();
            //加载插件
            DirectoryInfo info = new DirectoryInfo(_pluginPath);
            if (info.Exists)
            {
                //加载模块
                FileInfo[] tmodfiles = info.GetFiles();
                foreach (FileInfo fi in tmodfiles)
                {
                    if (".dll".Equals(fi.Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        PluginObject plg = _plugins.LoadPlugin(fi.FullName);
                        if (plg != null && _readAssetsFromPlugin)
                        {
                            UpdateAssetsAndDocument(plg);
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(_pluginPath);
            }
            //执行加载完成事件
            TAEventDispatcher.Instance.DispathLoadAfter(this);

            //开启监控插件更改
            _watcher = new FileSystemWatcher();
            _watcher.Filter = "*.dll";
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Path = _pluginPath;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += OnPluginListener;

            _timer = new Timer(new TimerCallback(OnWatchedFileChange),
                             null, Timeout.Infinite, Timeout.Infinite);
            return this;
        }

        #region 插件监听代码
        private static HashSet<string> _changePaths = new HashSet<string>();
        private void OnPluginListener(object sender, FileSystemEventArgs e)
        {
            string tpath = e.FullPath.ToLower();
            lock (_changePaths)
            {
                if (!_changePaths.Contains(tpath))
                {
                    _changePaths.Add(tpath);
                }
                _timer.Change(500, Timeout.Infinite);
            }
        }
        private void OnWatchedFileChange(object state)
        {
            List<string> backup = new List<string>();
            lock (_changePaths)
            {
                backup.AddRange(_changePaths);
                _changePaths.Clear();
            }
            foreach (string tpath in backup)
            {
                PluginObject obj = _plugins.LoadPlugin(tpath);
                if (obj != null)
                {
                    if (_readAssetsFromPlugin)
                    {
                        UpdateAssetsAndDocument(obj);
                    }
                    obj.Dispatcher.DispathLoadAfter(this);
                }
            }
            if (backup.Count > 0)
            {
                TemplateApp.Instance.CacheEmpty();
            }
        }
        #endregion
    }
}
