
namespace TemplateAction.Core
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public interface ITALoggerFactory
    {
        ITALogger CreateLogger(string categoryName);
    }
}
