
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface ITAServices
    {
        object GetService(string key);
        T GetService<T>() where T : class;
        List<T> GetServices<T>() where T : class;
    }
}
