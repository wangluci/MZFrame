using System;
using System.IO;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Label;
using TemplateAction.Extension.Site;
using System.Reflection;

namespace TemplateAction.Core
{
    /// <summary>
    /// web应用程序
    /// </summary>
    public class TASiteApplication : TAAbstractApplication
    {
        /// <summary>
        /// 是否从插件读取资源
        /// </summary>
        private bool _readAssetsFromPlugin;
        public bool ReadAssetsFromPlugin
        {
            get { return _readAssetsFromPlugin; }
        }
        private FilterCenter _filterCenter;
        internal FilterCenter Filters
        {
            get { return _filterCenter; }
        }
        /// <summary>
        /// 正在使用的中间件名
        /// </summary>
        private HashSet<string> _middlewareUsing;

        /// <summary>
        /// 参数绑定插件链表
        /// </summary>
        private LinkedList<IParamMapping> _mappingLink = new LinkedList<IParamMapping>();
        /// <summary>
        /// 首个参数绑定插件
        /// </summary>
        /// <returns></returns>
        public LinkedListNode<IParamMapping> FirstParamMapping()
        {
            return _mappingLink.First;
        }
        /// <summary>
        /// 新增参数绑定插件
        /// </summary>
        /// <param name="parser"></param>
        public void UseParamMapping(IParamMapping mapping)
        {
            _mappingLink.AddLast(mapping);
        }
        public TASiteApplication()
        {
            _filterCenter = new FilterCenter();
            _readAssetsFromPlugin = true;
            _middlewareUsing = new HashSet<string>();
            _mappingLink.AddLast(new DefatulParamMapping());
        }
        protected override IPluginCollectionExtData CreatePluginCollectionExtData()
        {
            return new SitePluginCollectionExtData();
        }
      
        /// <summary>
        /// 按顺序使用指定中间件，需要先AddSingle注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TASiteApplication UseMiddleware<T>() where T : class, IFilterMiddleware
        {
            string key = typeof(T).FullName;
            if (_middlewareUsing.Add(key))
            {
                _filterCenter.Add(new PluginMiddleware(key));
            }
            return this;
        }
        /// <summary>
        /// 将中间件放在第一个执行位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TASiteApplication UseMiddlewareFirst<T>() where T : class, IFilterMiddleware
        {
            string key = typeof(T).FullName;
            if (_middlewareUsing.Add(key))
            {
                _filterCenter.AddFirst(new PluginMiddleware(key));
            }
            return this;
        }
        /// <summary>
        /// 清空中间件
        /// </summary>
        /// <returns></returns>
        public TASiteApplication ClearMiddleware()
        {
            _filterCenter.Clear();
            return this;
        }
        /// <summary>
        /// 开始路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TAActionBuilder Route(ITAContext context)
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
                return CreateTARequestHandleBuilder(context, tns, tcontroller, taction, exparams);
            }
            return null;
        }
        public TAActionBuilder CreateTARequestHandleBuilder(ITAContext context, string ns, string controller, string action, ITAObjectCollection ext = null)
        {
            ControllerNode controllerNode = _plugins.GetControllerByKeyInPlugin(ns, controller);
            if (controllerNode == null)
                return null;
            ActionNode node = _plugins.GetMethodByKeyInPlugin(ns, controller, action);
            if (node == null)
                return null;
            return new TAActionBuilder(context, controllerNode, node, ext);
        }
        /// <summary>
        /// 使用指定路由创建工厂并加载全局路由
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public TASiteApplication UseRouterBuilder(IRouterBuilder builder)
        {
            _plugins.UseRouter(builder);
            return this;
        }
        /// <summary>
        /// 获取所有描述信息
        /// </summary>
        /// <returns></returns>
        public List<DescribeInfo> FindAllDescribe()
        {
            return _plugins.FindAllDescribe();
        }
        /// <summary>
        /// 调用者会从各个插件中复制资源文件
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public TASiteApplication UsePluginAssets(bool enable)
        {
            _readAssetsFromPlugin = enable;
            return this;
        }
        public TASiteApplication UsePluginPath(string path)
        {
            _pluginPath = path;
            return this;
        }
        public TASiteApplication Init(string rootpath, Assembly entry)
        {
            InitApplication(rootpath, entry);
            return this;
        }

        public bool ExistController(string controller)
        {
            return _plugins.ExistController(controller);
        }
        protected override void PluginLoad(PluginObject plg)
        {
            PushConcurrentTask(()=>{
                if (_readAssetsFromPlugin)
                {
                    UpdateDocument(plg);
                }
            });
            base.PluginLoad(plg);
        }
        protected override void BeforeInit()
        {
            //如果模板引擎未初始化，就使用默认路径初始化
            TemplateApp.Instance.Init(_rootPath);
            //激活配置文件
            TAEventDispatcher.Instance.DispathLoadBefore(this);
        }
        /// <summary>
        /// 更新模块资源
        /// </summary>
        private void UpdateDocument(PluginObject plg)
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
                        TemplateApp.Instance.AddViewPage(tmpstr, sr.ReadToEnd(), plg.CacheDependency);
                    }

                }


            }
        }
    }
}
