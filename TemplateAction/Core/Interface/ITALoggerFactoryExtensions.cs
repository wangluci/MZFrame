

namespace TemplateAction.Core.Interface
{
    public static class ITALoggerFactoryExtensions
    {
        public static ITALogger CreateLogger<T>(this ITALoggerFactory factory)
        {
            return factory.CreateLogger(typeof(T).ToString());
        }
    }
}
