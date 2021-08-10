
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IPluginFactory
    {
        PluginObject Create(Assembly tAssembly, string filepath);
    }
}
