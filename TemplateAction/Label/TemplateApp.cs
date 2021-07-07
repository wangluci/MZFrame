using System;
using System.Collections.Generic;
using System.Reflection;
using TemplateAction.Cache;
using TemplateAction.Common;
namespace TemplateAction.Label
{
    /// <summary>
    /// 提供模板扩展数据
    /// </summary>
    public class TemplateApp
    {
        internal static readonly TemplateApp _instance = new TemplateApp();
        static TemplateApp() { }
        /// <summary>
        /// 内置函数
        /// </summary>
        private Dictionary<string, MethodInfo> mSysMethods;
        /// <summary>
        /// 模板编译缓存
        /// </summary>
        private CachePool _pool;
        //模板监听器
        private FileDependencyWatcher _watcher;
        private TemplateApp()
        {
            _pool = new CachePool();
            mSysMethods = new Dictionary<string, MethodInfo>();
            //加载模板系统函数
            Type funType = typeof(TempFuns);
            MethodInfo[] methodArray = funType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methodArray)
            {
                mSysMethods.Add(method.Name, method);
            }
            _watcher = new FileDependencyWatcher(AppDomain.CurrentDomain.BaseDirectory, "*" + TAUtility.FILE_EXT);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void CacheEmpty()
        {
            _pool.Empty();
        }
        /// <summary>
        /// 添加模板原
        /// </summary>
        /// <param name="path"></param>
        /// <param name="input"></param>
        /// <param name="dep"></param>
        public void AddViewPage(string path, string input, FileDependency dep)
        {
            path = path.ToLower();
            TemplateDocument filedoc = new TemplateDocument(input);
            _pool.Remove(path);
            _pool.Insert(path, filedoc, new UnionOrDependency(dep, _watcher.CreateFileDependency(path)));
        }
        /// <summary>
        /// 加载模板原文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TemplateDocument LoadViewPage(string path)
        {
            path = path.ToLower();
            TemplateDocument filedoc = _pool.Get(path) as TemplateDocument;
            if (Equals(filedoc, null))
            {
                string filecont;
                int err = TAUtility.ReadFile(out filecont, path);
                if (err == 0)
                {
                    filedoc = new TemplateDocument(filecont);
                    FileDependency filedep = _watcher.CreateFileDependency(path);
                    if (filedep != null)
                    {
                        _pool.Insert(path, filedoc, filedep);
                    }
                }
                else
                {
                    return null;
                }
            }

            return filedoc;
        }

        /// <summary>
        /// 获取系统函数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MethodInfo GetSystemMethod(string name)
        {
            MethodInfo method;
            if (mSysMethods.TryGetValue(name, out method))
            {
                return method;
            }
            return null;
        }

        public static TemplateApp Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
