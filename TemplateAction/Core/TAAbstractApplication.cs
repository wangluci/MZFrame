using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 加载所有的插件
    /// </summary>
    public abstract class TAAbstractApplication : IDisposable, ITAApplication
    {
        /// <summary>
        /// 模块后缀名
        /// </summary>
        public const string ModExt = ".dll";
        protected bool _disposed = false;
        protected int _loaded = 0;
        /// <summary>
        /// 插件集
        /// </summary>
        protected PluginCollection _plugins;
        protected LinkedListNode<IDispatcher> _pluginsnode;
        //监控插件更改
        protected FileSystemWatcher _watcher;
        protected Timer _timer = null;
        protected string _rootPath;
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
        protected string _pluginPath;
        public string PluginPath
        {
            get { return _pluginPath; }
        }
        public IServiceCollection Services
        {
            get { return _plugins.Services; }
        }
        public ITAServices ServiceProvider
        {
            get { return _plugins; }
        }

        ~TAAbstractApplication()
        {
            Dispose(false);
        }
        public TAAbstractApplication()
        {
            _plugins = new PluginCollection();
            _pluginsnode = TAEventDispatcher.Instance.AddScope(_plugins);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) => LoadEmbeddedAssembly(e.Name);
        }
        /// <summary>
        /// 模块引用其它模块时调用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Assembly LoadEmbeddedAssembly(string name)
        {
            string[] tarr = name.Split(',');
            PluginObject plg = _plugins.GetPlugin(tarr[0].ToLower());
            if (plg == null)
            {
                string filepath = _rootPath + Path.DirectorySeparatorChar + name + ModExt;
                plg = _plugins.LoadPlugin(filepath);
                if (plg != null)
                {
                    AfterPluginChanged(plg);
                }
            }
            return plg.TargetAssembly;
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
        /// 插件更新时
        /// </summary>
        /// <param name="plg"></param>
        protected virtual void AfterPluginChanged(PluginObject plg)
        {
            plg.Config.Loaded(this, plg.Dispatcher);
        }
        /// <summary>
        /// 初始化前
        /// </summary>
        protected virtual void BeforeInit()
        {
            //激活配置文件
            TAEventDispatcher.Instance.DispathLoadBefore(this);
        }
        /// <summary>
        /// 初始化后
        /// </summary>
        protected virtual void AfterInit(List<PluginObject> plglist)
        {
            //执行加载完成事件
            foreach (PluginObject plg in plglist)
            {
                plg.Config.Loaded(this, plg.Dispatcher);
            }
        }
        /// <summary>
        /// 初始化并加载目录下的插件
        /// </summary>
        /// <param name="rootpath"></param>
        protected void InitApplication(string rootpath)
        {
            if (rootpath.Length > 0 && rootpath[rootpath.Length - 1] == Path.DirectorySeparatorChar)
            {
                rootpath = rootpath.Substring(0, rootpath.Length - 1);
            }
            if (_loaded == 1)
            {
                return;
            }
            int orists = Interlocked.CompareExchange(ref _loaded, 1, 0);
            if (orists != 0)
            {
                return;
            }
            //初始化根目录
            _rootPath = rootpath;
            //默认插件路径
            _pluginPath = Path.Combine(_rootPath, "Plugin");
            BeforeInit();
            //从应用程序域的程序集中初始化插件集
            _plugins.InitFromEntryAssembly();
            //加载插件
            List<PluginObject> tmpPlugins = new List<PluginObject>();
            DirectoryInfo info = new DirectoryInfo(_pluginPath);
            if (info.Exists)
            {
                //加载模块
                FileInfo[] tmodfiles = info.GetFiles();
                foreach (FileInfo fi in tmodfiles)
                {
                    if (ModExt.Equals(fi.Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        string filename = Path.GetFileName(fi.FullName);
                        if (_plugins.GetPlugin(filename.ToLower()) == null)
                        {
                            PluginObject plg = _plugins.LoadPlugin(fi.FullName);
                            if (plg != null)
                            {
                                tmpPlugins.Add(plg);
                            }
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(_pluginPath);
            }
            AfterInit(tmpPlugins);

            //开启监控插件更改
            _watcher = new FileSystemWatcher();
            _watcher.Filter = "*"+ ModExt;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Path = _pluginPath;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += OnPluginListener;

            _timer = new Timer(new TimerCallback(OnWatchedFileChange),
                             null, Timeout.Infinite, Timeout.Infinite);
            return;
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
                    AfterPluginChanged(obj);
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
