using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 路由模板特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RouteAttribute : Attribute
    {
        private string _template;
        public RouteAttribute(string template)
        {
            _template = template;
        }
        public string Template { get { return _template; } }
    }
}
