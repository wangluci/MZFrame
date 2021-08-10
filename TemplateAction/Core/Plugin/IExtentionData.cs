using System;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IExtentionData
    {
        void LoadBefore(IPluginFactory factory, Assembly assembly, string pluginpath);
        bool LoadItem(string pluginName,Type t);
        void LoadAfter(IPluginFactory factory, Assembly assembly, string pluginpath);
    }
}
