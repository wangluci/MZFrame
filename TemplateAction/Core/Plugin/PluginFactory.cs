using System;
using System.Reflection;

namespace TemplateAction.Core
{
    public class PluginFactory : IPluginFactory
    {
        public PluginObject Create(Assembly tAssembly, string filepath)
        {
            PluginObject newObj = new PluginObject(this, null, tAssembly, filepath);
            if (newObj.Config == null) return null;
            return newObj;
        }
    }
}
