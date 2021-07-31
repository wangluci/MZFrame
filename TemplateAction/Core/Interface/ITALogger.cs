
namespace TemplateAction.Core
{
    public interface ITALogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
