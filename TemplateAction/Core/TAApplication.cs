using System;

namespace TemplateAction.Core
{
    /// <summary>
    /// 控制台程序等应用
    /// </summary>
    public class TAApplication : TAAbstractApplication
    {
        public TAAbstractApplication UsePluginPath(string path)
        {
            _pluginPath = path;
            return this;
        }
        public TAApplication Init(string rootpath)
        {
            InitApplication(rootpath);
            return this;
        }
    }
}
