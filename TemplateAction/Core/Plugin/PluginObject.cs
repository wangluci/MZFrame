using System;
using System.Reflection;
using TemplateAction.Cache;

namespace TemplateAction.Core
{
    public class PluginObject
    {
        private string mName;
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// 当前插件版本
        /// </summary>
        private Version mVersion;

        private IServiceCollection _services;
        public IServiceCollection Services
        {
            get { return _services; }
        }
        /// <summary>
        /// 插件的局部事件分发器
        /// </summary>
        private PluginEventDispatcher _dispatcher;
        public PluginEventDispatcher Dispatcher
        {
            get { return _dispatcher; }
        }
        private ConcurrentStorer _storer;
        /// <summary>
        /// 插件单例存储
        /// </summary>
        public ConcurrentStorer Storer
        {
            get { return _storer; }
        }


        private IPluginConfig _config;
        public IPluginConfig Config
        {
            get { return _config; }
        }
        private Assembly _assembly;
        public Assembly TargetAssembly
        {
            get { return _assembly; }
        }

        private FileDependency _cacheDependency;
        public FileDependency CacheDependency
        {
            get { return _cacheDependency; }
        }
        private string _plgPath;
        public string PluginPath
        {
            get { return _plgPath; }
        }
        private static string PluginConfigName = typeof(IPluginConfig).FullName;

        private ExtentionDataCollection _data;
        /// <summary>
        /// 插件扩展数据
        /// </summary>
        public ExtentionDataCollection Data
        {
            get { return _data; }
        }

        public PluginObject(IPluginCollectionExtData pcdata, Assembly assembly, string pluginpath)
        {
            this._assembly = assembly;
            this._plgPath = pluginpath;
            this._cacheDependency = new FileDependency();
            this._storer = new ConcurrentStorer();
            this._dispatcher = new PluginEventDispatcher();
            this.mName = assembly.GetName().Name;
            this._services = new ServiceCollection(this.mName);
            this.mVersion = assembly.GetName().Version;
            this._data = new ExtentionDataCollection();

            if (pcdata != null)
            {
                pcdata.PluginLoadBefore(this);

                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (!pcdata.PluginLoadType(this, t))
                        {
                            if (t.GetInterface(PluginConfigName) != null)
                            {
                                //执行插件配置文件
                                this._config = Activator.CreateInstance(t) as IPluginConfig;
                                if (this._config != null)
                                {
                                    TAEventDispatcher.Instance.DispathPluginConfig(this);
                                }
                            }
                        }

                    }

                }
                pcdata.PluginLoadAfter(this);
            }
            else
            {
                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (t.GetInterface(PluginConfigName) != null)
                        {
                            //执行插件配置文件
                            this._config = Activator.CreateInstance(t) as IPluginConfig;
                            if (this._config != null)
                            {
                                TAEventDispatcher.Instance.DispathPluginConfig(this);
                            }
                        }
                    }

                }
            }
            TAEventDispatcher.Instance.DispathPluginLoad(this);
        }

        /// <summary>
        /// 查找服务
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public IServiceDescriptorEnumerable FindService(string key)
        {
            return _services[key];
        }

        /// <summary>
        /// 插件御载
        /// </summary>
        public void Unload()
        {
            TAEventDispatcher.Instance.DispathPluginUnload(this);
        }
    }
}
