namespace TemplateAction.Core
{
    public interface ITAEventHandler<T> where T : class
    {
        void OnEvent(T evt);
    }
}
