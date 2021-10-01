using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using TemplateAction.Common;
namespace TemplateAction.Core
{
    /// <summary>
    /// 插件集合
    /// </summary>
    public class PluginCollection : ITAServices, IDispatcher, IInstanceFactory
    {
        private Dictionary<string, PluginObject> mPluginList = new Dictionary<string, PluginObject>();
        private ReaderWriterLockSlim _lockslim = new ReaderWriterLockSlim();
        /// <summary>
        /// 单例服务实例
        /// </summary>
        private ConcurrentStorer _singletonServices;
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
        private IPluginCollectionExtData _extionData;
        public IPluginCollectionExtData ExtentionData
        {
            get { return _extionData; }
        }
        /// <summary>
        /// 单服务搜索缓存
        /// </summary>
        private ConcurrentDictionary<string, string> _keycache = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 多服务搜索缓存
        /// </summary>
        private ConcurrentDictionary<string, string[]> _mulkeycache = new ConcurrentDictionary<string, string[]>();
        public PluginCollection(IPluginCollectionExtData extionData)
        {
            _extionData = extionData;
            _singletonServices = new ConcurrentStorer();
            _services = new ServiceCollection(string.Empty);
        }
        private IServiceDescriptorEnumerable FindServicesIn(string key)
        {
            List<string> tnslist = new List<string>();
            ServiceDescriptorUnion deslist = new ServiceDescriptorUnion();
            IServiceDescriptorEnumerable gdes = _services[key];
            if (gdes != null)
            {
                deslist.Union(gdes);
                tnslist.Add("999");
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
                IServiceDescriptorEnumerable des = plg.FindService(key);
                if (des != null)
                {
                    deslist.Union(des);
                    tnslist.Add(plg.Name);
                }
            }
            _mulkeycache.TryAdd(key, tnslist.ToArray());
            return deslist;
        }
        /// <summary>
        /// 通过接口查找多个服务
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private IServiceDescriptorEnumerable FindServices(string key)
        {
            ServiceDescriptorUnion deslist = new ServiceDescriptorUnion();
            string[] nsArr;
            if (_mulkeycache.TryGetValue(key, out nsArr))
            {
                foreach (string ns in nsArr)
                {
                    if (ns.Equals("999"))
                    {
                        IServiceDescriptorEnumerable gdes = _services[key];
                        if (gdes != null)
                        {
                            deslist.Union(gdes);
                        }
                        else
                        {
                            _mulkeycache.TryRemove(key, out nsArr);
                            return FindServicesIn(key);
                        }
                    }
                    else
                    {
                        PluginObject pobj = GetPlugin(ns);
                        IServiceDescriptorEnumerable des = pobj.FindService(key);
                        if (des != null)
                        {
                            deslist.Union(des);
                        }
                        else
                        {
                            _mulkeycache.TryRemove(key, out nsArr);
                            return FindServicesIn(key);
                        }
                    }
                }
                return deslist;
            }
            return FindServicesIn(key);
        }
        private IServiceDescriptorEnumerable FindServiceIn(string key)
        {
            //查找全局服务
            IServiceDescriptorEnumerable returnDesc = _services[key];
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
        private IServiceDescriptorEnumerable FindService(string key)
        {
            IServiceDescriptorEnumerable returnDesc;
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

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetService(string key, ILifetimeFactory scopeFactory = null)
        {
            IServiceDescriptorEnumerable sd = FindService(key);
            if (sd == null)
            {
                return null;
            }
            return Des2Instance(sd.First, scopeFactory);
        }
        public object GetService(Type tp, ILifetimeFactory scopeFactory = null)
        {
            if (tp.IsGenericType && !tp.IsGenericTypeDefinition)
            {
                Type defType = tp.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>) == defType)
                {
                    //获取服务集
                    Type destType = tp.GetGenericArguments()[0];
                    Type listType = typeof(List<>).MakeGenericType(destType);
                    object list = Activator.CreateInstance(listType);
                    IServiceDescriptorEnumerable sdenum = FindServices(destType.FullName);
                    MethodInfo addMethod = listType.GetMethod("Add");
                    foreach (ServiceDescriptor sd in sdenum)
                    {
                        if (sd != null)
                        {
                            addMethod.Invoke(list, new object[] { Des2Instance(sd, scopeFactory) });
                        }
                    }
                    return list;
                }
                else
                {
                    IServiceDescriptorEnumerable sdenum = FindService(tp.FullName);
                    if (sdenum == null)
                    {
                        //使用泛型定义搜索
                        sdenum = FindService(tp.GetGenericTypeDefinition().FullName);
                        if (sdenum == null) return null;
                        ServiceDescriptor sd = sdenum.First;
                        return Des2Instance(sd.PluginName, sd.Lifetime, sd.ServiceType.MakeGenericType(tp.GetGenericArguments()), sd.Factory, sd.Instance, scopeFactory);
                    }
                    return Des2Instance(sdenum.First, scopeFactory);
                }

            }
            else
            {
                IServiceDescriptorEnumerable sdenum = FindService(tp.FullName);
                if (sdenum == null) return null;
                return Des2Instance(sdenum.First, scopeFactory);
            }
        }

        /// <summary>
        /// 创建外部注入服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        public object CreateScopeService(ILifetimeFactory scopeFactory, Type serviceType)
        {
            return scopeFactory.GetValue(this, serviceType, null);
        }
        public object CreateScopeService(ILifetimeFactory scopeFactory, ProxyFactory factory)
        {
            return scopeFactory.GetValue(this, null, factory);
        }
        private object Des2Instance(ServiceDescriptor sd, ILifetimeFactory scopeFactory)
        {
            return Des2Instance(sd.PluginName, sd.Lifetime, sd.ServiceType, sd.Factory, sd.Instance, scopeFactory);
        }

        /// <summary>
        /// ServiceDescriptor转实例
        /// </summary>
        /// <param name="plgname"></param>
        /// <param name="lifetime"></param>
        /// <param name="serviceType"></param>
        /// <param name="factory"></param>
        /// <param name="impInstance"></param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        private object Des2Instance(string plgname, ServiceLifetime lifetime, Type serviceType, ProxyFactory factory, object impInstance, ILifetimeFactory scopeFactory)
        {
            if (impInstance != null) return impInstance;
            object result = null;
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        if (string.IsNullOrEmpty(plgname))
                        {
                            ConcurrentProxy proxy = this._singletonServices.GetOrAdd(serviceType.FullName);
                            result = proxy.GetValue(this, serviceType, factory, scopeFactory);
                        }
                        else
                        {
                            PluginObject pobj = GetPlugin(plgname);
                            if (!Equals(pobj, null))
                            {
                                ConcurrentProxy proxy = pobj.Storer.GetOrAdd(serviceType.FullName);
                                result = proxy.GetValue(this, serviceType, factory, scopeFactory);
                            }
                        }
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        result = CreateServiceInstance(serviceType, factory, scopeFactory);
                    }
                    break;
                case ServiceLifetime.Scope:
                    {
                        if (scopeFactory != null)
                        {
                            result = scopeFactory.GetValue(this, serviceType, factory);
                        }
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
        public object CreateServiceInstance(Type serviceType, ProxyFactory factory, ILifetimeFactory scopeFactory)
        {
            if (serviceType == null) return factory(new object[0], this);
            //接口则直接调用factory无参构造
            if (serviceType.IsInterface && factory != null)
            {
                return factory(new object[0], this);
            }
            else if (serviceType.IsPrimitive || serviceType == typeof(string))
            {
                //string或基本类型返回
                if (factory != null)
                {
                    return factory(new object[0], this);
                }
                else
                {
                    return Activator.CreateInstance(serviceType);
                }
            }

            ConstructorInfo[] constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length > 0)
            {
                //开始选择构造函数
                ConstructorInfo activationConstructor = null;
                object[] parameters = null;
                string err_param_name = null;
                foreach (ConstructorInfo tmpConstructor in constructors)
                {
                    err_param_name = null;
                    ParameterInfo[] tmpParameterInfos = tmpConstructor.GetParameters();
                    object[] tmpParameters = new object[tmpParameterInfos.Length];

                    for (int i = 0; i < tmpParameterInfos.Length; i++)
                    {
                        ParameterInfo parameter = tmpParameterInfos[i];
                        Type parameterType = parameter.ParameterType;

                        if (parameterType == typeof(ITAServices))
                        {
                            tmpParameters[i] = this;
                        }
                        else if (parameterType.IsPrimitive || parameterType == typeof(string))
                        {
                            tmpParameters[i] = GetService(TAUtility.TypeName2ServiceKey(parameterType, parameter.Name), scopeFactory);
                        }
                        else
                        {
                            tmpParameters[i] = GetService(parameterType, scopeFactory);
                            if (tmpParameters[i] == null)
                            {
                                if (parameter.HasDefaultValue)
                                {
                                    tmpParameters[i] = parameter.DefaultValue;
                                }
                                else
                                {
                                    err_param_name = parameter.Name;
                                    break;
                                }
                            }
                        }
                    }
                    if (err_param_name == null)
                    {
                        if (activationConstructor == null)
                        {
                            activationConstructor = tmpConstructor;
                            parameters = tmpParameters;
                        }
                        else if (parameters.Length < tmpParameters.Length)
                        {
                            activationConstructor = tmpConstructor;
                            parameters = tmpParameters;
                        }
                    }
                }

                if (activationConstructor == null)
                {
                    throw new ArgumentException(string.Format("{0} 构造函数的参数无法注入,请确保 {1} 已加入服务容器", serviceType.Name, err_param_name));
                }

                if (factory != null)
                {
                    return factory(parameters, this);
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
                    return factory(new object[0], this);
                }
                else
                {
                    return Activator.CreateInstance(serviceType);
                }
            }
        }
        /// <summary>
        /// 判断是否存在指定插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool ExistPlugin(string ns)
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
        /// <summary>
        /// 获取所有插件
        /// </summary>
        /// <returns></returns>
        public PluginObject[] GetAllPlugin()
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
            return tarr;
        }
        /// <summary>
        /// 获取指定插件
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public PluginObject GetPlugin(string ns)
        {
            _lockslim.EnterReadLock();
            try
            {
                PluginObject pobj = null;
                if (mPluginList.TryGetValue(ns.ToLower(), out pobj))
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
        private PluginObject Create(Assembly tAssembly, string filepath)
        {
            PluginObject newObj = new PluginObject(_extionData, tAssembly, filepath);
            if (newObj.Config == null) return null;
            return newObj;
        }
        /// <summary>
        /// 创建入口插件
        /// </summary>
        public void InitFromEntryAssembly()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            if (ass != null)
            {
                PluginObject newObj = Create(ass, string.Empty);
                if (newObj != null)
                {
                    mPluginList[newObj.Name.ToLower()] = newObj;
                    TAEventDispatcher.Instance.DispathPluginLoad(newObj);
                }
            }
        }
        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="tAssembly"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public PluginObject CreatePlugin(Assembly tAssembly, string filepath)
        {
            if (tAssembly == null) return null;
            PluginObject newObj = Create(tAssembly, filepath);
            if (newObj != null)
            {
                PluginObject oldPlugin;
                _lockslim.EnterWriteLock();
                try
                {
                    string keylow = newObj.Name.ToLower();
                    mPluginList.TryGetValue(keylow, out oldPlugin);
                    mPluginList[keylow] = newObj;
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

                TAEventDispatcher.Instance.DispathPluginLoad(newObj);
            }

            return newObj;
        }
        /// <summary>
        /// 移除指定插件
        /// </summary>
        /// <param name="ns"></param>
        public void RemovePlugin(string ns)
        {
            PluginObject oldPlugin;
            _lockslim.EnterWriteLock();
            try
            {
                mPluginList.TryGetValue(ns.ToLower(), out oldPlugin);
                mPluginList.Remove(oldPlugin.Name.ToLower());
            }
            finally
            {
                _lockslim.ExitWriteLock();
            }
            if (oldPlugin != null)
            {
                oldPlugin.CacheDependency.NoticeChange();
                oldPlugin.Unload();
                try
                {
                    File.Delete(oldPlugin.PluginPath);
                }
                catch { }
            }
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
