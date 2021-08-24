using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using TemplateAction.Common;
using TemplateAction.Extension;
namespace TemplateAction.Core
{
    /// <summary>
    /// 插件集合
    /// </summary>
    public class PluginCollection : ITAServices, IDispatcher
    {
        private ConcurrentDictionary<string, string> _keycache = new ConcurrentDictionary<string, string>();
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
        private IPluginFactory _pluginFactory;
        public IPluginFactory PluginFactory
        {
            get { return _pluginFactory; }
        }

        public PluginCollection(IPluginFactory factory)
        {
            _pluginFactory = factory;
            _singletonServices = new ConcurrentStorer();
            _services = new ServiceCollection(string.Empty);
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
        public List<T> GetServices<T>(ILifetimeFactory extOtherFactory = null) where T : class
        {
            Type listType = typeof(List<>).MakeGenericType(typeof(T));
            object list = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            List<ServiceDescriptor> deslist = FindServices(typeof(T).FullName);
            foreach (ServiceDescriptor sd in deslist)
            {
                addMethod.Invoke(list, new object[] { Des2Instance(sd, extOtherFactory) });
            }

            return list as List<T>;
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetService(string key, ILifetimeFactory extOtherFactory = null)
        {
            ServiceDescriptor sd = FindService(key);
            return Des2Instance(sd, extOtherFactory);
        }
        /// <summary>
        /// 创建外部注入服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="extOtherFactory"></param>
        /// <returns></returns>
        public object CreateExtOtherService(Type serviceType, ILifetimeFactory extOtherFactory, ProxyFactory factory = null)
        {
            return extOtherFactory.GetValue(this, serviceType, factory, extOtherFactory);
        }

        /// <summary>
        /// ServiceDescriptor转实例
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        private object Des2Instance(ServiceDescriptor sd, ILifetimeFactory extOtherFactory)
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
                            result = proxy.GetValue(this, sd, extOtherFactory);
                        }
                        else
                        {
                            PluginObject pobj = GetPlugin(sd.PluginName);
                            if (!Equals(pobj, null))
                            {
                                ConcurrentProxy proxy = pobj.Storer.GetOrAdd(sd.ServiceType.FullName);
                                result = proxy.GetValue(this, sd, extOtherFactory);
                            }
                        }
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        result = CreateServiceInstance(sd.ServiceType, sd.Factory, extOtherFactory);
                    }
                    break;
                case ServiceLifetime.Other:
                    {
                        if (sd.LifetimeFactory != null)
                        {
                            result = sd.LifetimeFactory.GetValue(this, sd.ServiceType, sd.Factory, extOtherFactory);
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
        public object CreateServiceInstance(Type serviceType, ProxyFactory factory, ILifetimeFactory extOtherFactory)
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
                    else if (parameterType.IsPrimitive || parameterType == typeof(string))
                    {
                        ;
                        parameters[i] = this.GetService(TAUtility.TypeName2ServiceKey(parameterType, parameter.Name), extOtherFactory);
                    }
                    else
                    {
                        parameters[i] = this.GetService(parameterType, extOtherFactory);
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
        public void InitFromEntryAssembly()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            if (ass != null)
            {
                PluginObject newObj = _pluginFactory.Create(ass, string.Empty);
                if (newObj != null)
                {
                    mPluginList[newObj.Name.ToLower()] = newObj;
                }
            }
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
        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="tAssembly"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public PluginObject CreatePlugin(Assembly tAssembly, string filepath)
        {
            if (tAssembly == null) return null;
            PluginObject newObj = _pluginFactory.Create(tAssembly, filepath);
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
