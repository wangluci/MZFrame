using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
namespace MyAccess.Aop
{
    /// <summary>
    /// Aop代理工厂
    /// </summary>
    public class InterceptFactory
    {
        private static ConcurrentDictionary<string, ProxyGenerator> _cacheProxyGenerator = new ConcurrentDictionary<string, ProxyGenerator>();
        public static ProxyGenerator GetProxyGenerator(Type t)
        {
            string tfullname = t.Assembly.GetHashCode().ToString();
            return _cacheProxyGenerator.GetOrAdd(tfullname, (string k) =>
            {
                return new ProxyGenerator();
            });
        }
        /// <summary>
        /// 创建DAL代理
        /// </summary>
        /// <param name="interfaceToProxy"></param>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateDAL(Type interfaceToProxy, Type t, object[] constructorArguments)
        {
            ProxyGenerator tpg = GetProxyGenerator(t);
            object target = Activator.CreateInstance(t, constructorArguments);
            return tpg.CreateInterfaceProxyWithTarget(interfaceToProxy, target, new DBIntercept());
        }

        /// <summary>
        /// 无接口创建DAL代理
        /// </summary>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateDAL(Type t, object[] constructorArguments)
        {
            ProxyGenerator tpg = GetProxyGenerator(t);
            return tpg.CreateClassProxy(t, constructorArguments, new DBIntercept());
        }

        /// <summary>
        /// 创建BLL代理
        /// </summary>
        /// <param name="interfaceToProxy"></param>
        /// <param name="t"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        public static object CreateBLL(Type interfaceToProxy, Type t, object[] constructorArguments)
        {
            ProxyGenerator tpg = GetProxyGenerator(t);
            object target = Activator.CreateInstance(t, constructorArguments);
            return tpg.CreateInterfaceProxyWithTarget(interfaceToProxy, target, new BLLIntercept());
        }
        /// <summary>
        /// 无接口创建BLL代理
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object CreateBLL(Type t, object[] constructorArguments)
        {
            ProxyGenerator tpg = GetProxyGenerator(t);
            return tpg.CreateClassProxy(t, constructorArguments, new BLLIntercept());
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateEntityOp<T>() where T : class
        {
            ProxyGenerationOptions option = new ProxyGenerationOptions();
            BaseEntity be = new BaseEntity();
            option.AddMixinInstance(be);
            ProxyGenerator tpg = GetProxyGenerator(typeof(T));
            T rt = tpg.CreateClassProxy<T>(option, new EntitylIntercept());
            be.StartRecord();
            return rt;
        }


        /// <summary>
        /// 创建事件监听器的触发实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callFun">事件触发函数</param>
        /// <returns></returns>
        public static T CreateDispatcher<T>(Action<string,object[]> callFun) where T : class
        {
            Type interfaceT = typeof(T);
            ProxyGenerator tpg = GetProxyGenerator(interfaceT);
            return tpg.CreateInterfaceProxyWithoutTarget<T>(new ListenerIntercept(callFun));
        }
    }
}
