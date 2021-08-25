using System;
using System.Collections.Generic;
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
        private IExtentionData _data;
        public IExtentionData Data { get { return _data; } }

        public PluginObject(IPluginFactory factory, IExtentionData data, Assembly assembly, string pluginpath)
        {
            this._data = data;
            this._assembly = assembly;
            this._plgPath = pluginpath;

            this._cacheDependency = new FileDependency();
            this._storer = new ConcurrentStorer();
            this._dispatcher = new PluginEventDispatcher();
            this.mName = assembly.GetName().Name;
            this._services = new ServiceCollection(this.mName);
            this.mVersion = assembly.GetName().Version;

            if (data != null)
            {
                data.LoadBefore(factory, assembly, pluginpath);
                string myPluginConfigName = typeof(IPluginConfig).FullName;

                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (!data.LoadItem(this.mName, t))
                        {
                            if (t.GetInterface(myPluginConfigName) != null)
                            {
                                //执行插件配置文件
                                this._config = Activator.CreateInstance(t) as IPluginConfig;
                                if (this._config != null)
                                {
                                    this._config.Configure(this._services);
                                }
                            }
                        }

                    }

                }
                data.LoadAfter(factory, assembly, pluginpath);
            }
            else
            {
                string myPluginConfigName = typeof(IPluginConfig).FullName;

                Type[] exports = this._assembly.GetExportedTypes();
                foreach (Type t in exports)
                {
                    //判断非抽像
                    if (!t.IsAbstract)
                    {
                        if (t.GetInterface(myPluginConfigName) != null)
                        {
                            //执行插件配置文件
                            this._config = Activator.CreateInstance(t) as IPluginConfig;
                            if (this._config != null)
                            {
                                this._config.Configure(this._services);
                            }
                        }
                    }

                }
            }

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
            if (_config != null)
            {
                _config.Unload();
            }
            TAEventDispatcher.Instance.DispathPluginUnload(this);
        }
    }
}
