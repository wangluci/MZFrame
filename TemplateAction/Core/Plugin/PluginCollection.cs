using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
namespace TemplateAction.Core
{
    /// <summary>
    /// 插件集合
    /// </summary>
    public class PluginCollection : IRouter, ITAServices, IDispatcher
    {
        private ConcurrentDictionary<string, string> _keycache = new ConcurrentDictionary<string, string>();
        private Dictionary<string, PluginObject> mPluginList = new Dictionary<string, PluginObject>();
        private ReaderWriterLockSlim _lockslim = new ReaderWriterLockSlim();
        /// <summary>
        /// 单例服务实例
        /// </summary>
        private ConcurrentStorer _singletonServices;
        /// <summary>
        /// 线程服务实例
        /// </summary>
        [ThreadStatic]
        private static SimpleStorer _threadServices = null;
        /// <summary>
        /// 全局服务
        /// </summary>
        private IServiceCollection _services;
        public IServiceCollection Services
        {
            get { return _services; }
        }
        /// <summary>
        /// 单例服务实例
        /// </summary>
        public ConcurrentStorer SingletonServices
        {
            get { return _singletonServices; }
        }
        private IRouterCollection _routerCollection;
        /// <summary>
        /// 路由创建工厂
        /// </summary>
        private IRouterBuilder _routerBuilder;
        internal IRouterBuilder RouterBuilder
        {
            get { return _routerBuilder; }
        }
        public PluginCollection()
        {
            _singletonServices = new ConcurrentStorer(this);
            _services = new ServiceCollection(string.Empty);
        }
        public List<T> GetServices<T>() where T : class
        {
            Type listType = typeof(List<>).MakeGenericType(typeof(T));
            object list = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            List<ServiceDescriptor> deslist = FindServices(typeof(T).FullName);
            foreach (ServiceDescriptor sd in deslist)
            {
                addMethod.Invoke(list, new object[] { Des2Instance(sd) });
            }

            return list as List<T>;
        }
 
        public T GetService<T>() where T : class
        {
            return GetService(typeof(T).FullName) as T;
        }
        public object GetService(Type tp)
        {
            return GetService(tp.FullName);
        }
        public void UseRouter(IRouterBuilder builder)
        {
            _routerBuilder = builder;
            _routerCollection = _routerBuilder.Build();
        }
        /// <summary>
        /// 路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IDictionary<string, object> Route(ITAContext context)
        {
            if (_routerCollection == null)
            {
                return null;
            }
            //先路由全局
            IDictionary<string, object> rt = _routerCollection.Route(context);
            if (rt != null)
            {
                return rt;
            }
            //再路由插件
            PluginObject[] tarr = null;
            _lockslim.EnterReadLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (tarr == null)
            {
                tarr = new PluginObject[0];
            }
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
        public List<AnnotationInfo> ViewAnnList()
        {
            _lockslim.EnterReadLock();
            try
            {
                List<AnnotationInfo> rtlist = new List<AnnotationInfo>();
                foreach (KeyValuePair<string, PluginObject> kvp in mPluginList)
                {
                    Dictionary<string, ControllerNode> ml = kvp.Value.GetControllerList();
                    foreach (KeyValuePair<string, ControllerNode> kpcn in ml)
                    {
                        if (!string.IsNullOrEmpty(kpcn.Value.Descript))
                        {
                            AnnotationInfo ai = new AnnotationInfo();
                            ai.Name = kpcn.Value.Descript;
                            ai.Code = string.Format("/{0}/{1}", kvp.Key, kpcn.Key);
                            ai.ParentCode = string.Empty;
                            rtlist.Add(ai);
                            foreach (KeyValuePair<string, Node> kpn in kpcn.Value.Childrens)
                            {
                                ActionNode an = kpn.Value as ActionNode;
                                if (an == null) continue;
                                if (!string.IsNullOrEmpty(kpn.Value.Descript))
                                {
                                    AnnotationInfo aii = new AnnotationInfo();
                                    aii.Name = kpn.Value.Descript;
                                    aii.Code = string.Format("{0}/{1}", ai.Code, kpn.Key);
                                    aii.ParentCode = kpcn.Key;
                                    rtlist.Add(aii);
                                }
                            }
                        }
                    }
                }
                return rtlist;
            }
            finally
            {
                _lockslim.ExitReadLock();
            }

        }
        /// <summary>
        /// 通过接口查找多个服务
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<ServiceDescriptor> FindServices(string key)
        {
            List<ServiceDescriptor> deslist = new List<ServiceDescriptor>();
            ServiceDescriptor gdes = _services[key];
            if (gdes != null)
            {
                deslist.Add(gdes);
            }
            PluginObject[] tarr = null;
            _lockslim.EnterReadLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (tarr == null)
            {
                tarr = new PluginObject[0];
            }
            foreach (PluginObject plg in tarr)
            {
                ServiceDescriptor des = plg.FindService(key);
                if (des != null)
                {
                    deslist.Add(des);
                }
            }
            return deslist;
        }
        private ServiceDescriptor FindServiceIn(string key)
        {
            //查找全局服务
            ServiceDescriptor returnDesc = _services[key];
            if (returnDesc != null)
            {
                _keycache.TryAdd(key, "999");
                return returnDesc;
            }
            //在其它的插件中查找
            PluginObject[] tarr = null;
            _lockslim.EnterReadLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (tarr == null)
            {
                tarr = new PluginObject[0];
            }
            foreach (PluginObject plg in tarr)
            {
                returnDesc = plg.FindService(key);
                if (returnDesc != null)
                {
                    _keycache.TryAdd(key, plg.Name);
                    return returnDesc;
                }
            }
            return null;
        }
        /// <summary>
        /// 通过接口查找单个服务
        /// </summary>
        /// <param name="key">接口类型</param>
        /// <returns>服务描述信息</returns>
        private ServiceDescriptor FindService(string key)
        {
            ServiceDescriptor returnDesc;
            string targetns;
            if (_keycache.TryGetValue(key, out targetns))
            {
                if (targetns.Equals("999"))
                {
                    returnDesc = _services[key];
                    if (returnDesc != null)
                    {
                        return returnDesc;
                    }
                    else
                    {
                        _keycache.TryRemove(key, out targetns);
                        return FindServiceIn(key);
                    }
                }
                else
                {
                    PluginObject pobj = GetPlugin(targetns);
                    returnDesc = pobj.FindService(key);
                    if (returnDesc != null)
                    {
                        return returnDesc;
                    }
                    else
                    {
                        _keycache.TryRemove(key, out targetns);
                        return FindServiceIn(key);
                    }
                }
            }
            else
            {
                return FindServiceIn(key);
            }

        }
        public void ClearThreadService()
        {
            if (!Equals(_threadServices, null))
            {
                _threadServices = null;
            }
        }
        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetService(string key)
        {
            ServiceDescriptor sd = FindService(key);
            return Des2Instance(sd);
        }
        public object CreateServiceInstance(Type serviceType)
        {
            return CreateServiceInstance(serviceType, null);
        }

        /// <summary>
        /// ServiceDescriptor转实例
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        private object Des2Instance(ServiceDescriptor sd)
        {
            object result = null;
            if (Equals(sd, null))
            {
                return null;
            }
            switch (sd.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        if (string.IsNullOrEmpty(sd.PluginName))
                        {
                            ConcurrentProxy proxy = this._singletonServices.GetOrAdd(sd.ServiceType.FullName);
                            result = proxy.GetValue(sd);
                        }
                        else
                        {
                            PluginObject pobj = GetPlugin(sd.PluginName);
                            if (!Equals(pobj, null))
                            {
                                ConcurrentProxy proxy = pobj.Storer.GetOrAdd(sd.ServiceType.FullName);
                                result = proxy.GetValue(sd);
                            }
                        }
                    }
                    break;
                case ServiceLifetime.Thread:
                    {
                        if (Equals(_threadServices, null))
                        {
                            _threadServices = new SimpleStorer();
                        }
                        result = _threadServices.GetInstance(sd.ServiceType.FullName);
                        if (Equals(result, null))
                        {
                            result = CreateServiceInstance(sd.ServiceType, sd.Factory);
                            _threadServices.AddInstance(sd.ServiceType.FullName, result);
                        }
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        result = CreateServiceInstance(sd.ServiceType, sd.Factory);
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object CreateServiceInstance(Type serviceType, ProxyFactory factory)
        {
            //接口则直接调用factory无参构造
            if (serviceType.IsInterface && factory != null)
            {
                return factory(new object[0]);
            }
            ConstructorInfo activationConstructor = null;
            ConstructorInfo[] constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length > 0)
            {
                ParameterInfo[] parameterInfos = null;
                for (int i = 0; i < constructors.Length; i++)
                {
                    activationConstructor = constructors[i];
                    parameterInfos = activationConstructor.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        break;
                    }
                }

                object[] parameters = new object[parameterInfos.Length];
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    ParameterInfo parameter = parameterInfos[i];
                    Type parameterType = parameter.ParameterType;
                    if (parameterType == typeof(ITAServices))
                    {
                        parameters[i] = this;
                    }
                    else
                    {
                        parameters[i] = GetService(parameterType);
                    }
                }
                if (factory != null)
                {
                    return factory(parameters);
                }
                else
                {
                    return activationConstructor.Invoke(parameters);
                }
            }
            else
            {
                if (factory != null)
                {
                    return factory(new object[0]);
                }
                else
                {
                    return Activator.CreateInstance(serviceType);
                }
            }
        }

        /// <summary>
        /// 获取指定插件的控制器
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Type GetControllerByKeyInPlugin(string ns, string key)
        {
            PluginObject pobj = GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetControllerByKey(key);
            }
            return null;
        }

        public ControllerNode GetControllerNodeByKeyInPlugin(string ns, string controller)
        {
            PluginObject pobj = GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetControllerNodeByKey(controller);
            }
            return null;
        }

        public ActionNode GetMethodByKeyInPlugin(string controller, string action, string ns)
        {
            PluginObject pobj = GetPlugin(ns);
            if (pobj != null)
            {
                return pobj.GetMethodByKey(controller, action);
            }
            return null;
        }
        public bool ContainController(string key)
        {
            _lockslim.EnterReadLock();
            try
            {
                foreach (KeyValuePair<string, PluginObject> kvp in mPluginList)
                {
                    if (kvp.Value.ContainController(key))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }
        public bool ContainPlugin(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                return mPluginList.ContainsKey(ns);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }
        public PluginObject GetPlugin(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                PluginObject pobj = null;
                if (mPluginList.TryGetValue(ns, out pobj))
                {
                    return pobj;
                }
                return null;
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
        }
        public void InitFromEntryAssembly()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            if (ass != null)
            {
                PluginObject newObj = PluginObject.Create(this, ass);
                if (newObj != null)
                {
                    mPluginList[newObj.Name] = newObj;
                }
            }
        }
        public PluginObject LoadPlugin(string path)
        {
            Assembly tAssembly = Assembly.Load(System.IO.File.ReadAllBytes(path));
            PluginObject newObj = PluginObject.Create(this, tAssembly);
            if (newObj != null)
            {
                PluginObject oldPlugin;
                _lockslim.EnterWriteLock();
                try
                {
                    mPluginList.TryGetValue(newObj.Name, out oldPlugin);
                    mPluginList[newObj.Name] = newObj;
                }
                finally
                {
                    _lockslim.ExitWriteLock();
                }
                if (oldPlugin != null)
                {
                    oldPlugin.CacheDependency.NoticeChange();
                    oldPlugin.Unload();
                }
            }
            return newObj;
        }

        public void Dispatch<T>(string key, T evt) where T : class
        {
            PluginObject[] tarr = null;
            _lockslim.EnterReadLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (tarr == null)
            {
                tarr = new PluginObject[0];
            }
            foreach (PluginObject plg in tarr)
            {
                plg.Dispatcher.Dispatch(key, evt);
            }
        }
        public bool IsExistHandler(string key)
        {
            PluginObject[] tarr = null;
            _lockslim.EnterReadLock();
            try
            {
                tarr = new PluginObject[mPluginList.Count];
                mPluginList.Values.CopyTo(tarr, 0);
            }
            finally
            {
                _lockslim.ExitReadLock();
            }
            if (tarr == null)
            {
                return false;
            }
            foreach (PluginObject plg in tarr)
            {
                if (plg.Dispatcher.IsExistHandler(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
