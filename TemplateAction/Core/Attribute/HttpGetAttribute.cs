using System;
namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class HttpGetAttribute : Attribute
    {
    }
}
