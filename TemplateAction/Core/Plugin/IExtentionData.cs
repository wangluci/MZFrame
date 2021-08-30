using System;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IExtentionData
    {
        void LoadBefore(IPluginCollectionExtData pcdata, Assembly assembly, string pluginpath);
        bool LoadItem(string pluginName,Type t);
        void LoadAfter(IPluginCollectionExtData pcdata, Assembly assembly, string pluginpath);
    }
}
