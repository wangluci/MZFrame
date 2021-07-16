using System;
using System.Collections.Generic;
using System.Reflection;
using TemplateAction.Cache;

namespace TemplateAction.Core
{
    public class PluginObject : IRouter
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
        private Dictionary<string, ControllerNode> mControllerList;
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
        public ConcurrentStorer Storer
        {
            get { return _storer; }
        }

        /// <summary>
        /// 路由创建工厂
        /// </summary>
        private IPluginRouterBuilder _routerBuilder;
        internal IPluginRouterBuilder RouterBuilder
        {
            get { return _routerBuilder; }
        }
        private IRouterCollection _routerCollection;
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
        public static PluginObject Create(PluginCollection collection, Assembly assembly)
        {

            PluginObject pluginObj = new PluginObject();
            pluginObj._assembly = assembly;
            if (collection.RouterBuilder != null)
            {
                pluginObj._routerBuilder = collection.RouterBuilder.NewPluginBuilder();
            }

            pluginObj._cacheDependency = new FileDependency();
            pluginObj._storer = new ConcurrentStorer(collection);
            pluginObj.mControllerList = new Dictionary<string, ControllerNode>();
            pluginObj._dispatcher = new PluginEventDispatcher();
            string myControllerName = typeof(IController).FullName;
            string myPluginConfigName = typeof(IPluginConfig).FullName;
            pluginObj.mName = assembly.GetName().Name;
            pluginObj._services = new ServiceCollection(pluginObj.mName);
            pluginObj.mVersion = assembly.GetName().Version;
            Type[] exports = assembly.GetExportedTypes();
            foreach (Type t in exports)
            {
                //判断非抽像
                if (!t.IsAbstract)
                {
                    if (t.GetInterface(myControllerName) != null)
                    {
                        //获取插件的控制器节点数据
                        ControllerNode n = new ControllerNode(pluginObj, t);
                        pluginObj.mControllerList[n.Key.ToLower()] = n;
                    }
                    else if (t.GetInterface(myPluginConfigName) != null)
                    {
                        //执行插件配置文件
                        pluginObj._config = Activator.CreateInstance(t) as IPluginConfig;
                        if (pluginObj._config != null)
                        {
                            pluginObj._config.Configure(pluginObj._services);
                        }
                    }
                }

            }
            if (pluginObj.mControllerList.Count == 0 && pluginObj._config == null)
                return null;
            if (pluginObj._routerBuilder != null)
            {
                pluginObj._routerCollection = pluginObj._routerBuilder.Build();
            }
            return pluginObj;
        }
        private PluginObject() { }
        public Dictionary<string, ControllerNode> GetControllerList()
        {
            return mControllerList;
        }
        /// <summary>
        /// 查找服务
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public ServiceDescriptor FindService(string key)
        {
            return _services[key];
        }
        public bool ContainController(string key)
        {
            return mControllerList.ContainsKey(key);
        }
        public ControllerNode GetControllerNodeByKey(string controller)
        {
            ControllerNode rtVal = null;

            if (mControllerList.TryGetValue(controller.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public ActionNode GetMethodByKey(string controller, string action)
        {
            ControllerNode rtVal = null;
            if (mControllerList.TryGetValue(controller.ToLower(), out rtVal))
            {
                ActionNode an = rtVal.GetChildNode(action) as ActionNode;
                if (an != null)
                {
                    return an;
                }
            }
            return null;
        }

        public IDictionary<string, object> Route(ITAContext context)
        {
            if (_routerCollection == null)
            {
                return null;
            }
            return _routerCollection.Route(context);
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
        }
    }
}
