using System;

namespace TemplateAction.Core
{
    public static class TAApplicationExtensions
    {
        public static TAApplication UsePluginPath(this TAApplication app, string path)
        {
            app.PluginPath = path;
            return app;
        }
        public static TAApplication Init(this TAApplication app, string rootpath)
        {
            app.TAInit(rootpath);
            return app;
        }
    }
}
