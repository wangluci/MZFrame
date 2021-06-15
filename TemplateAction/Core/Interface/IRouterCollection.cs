
namespace TemplateAction.Core
{
    public interface IRouterCollection : IRouter
    {
        void Add(IRouter router);
    }
}
