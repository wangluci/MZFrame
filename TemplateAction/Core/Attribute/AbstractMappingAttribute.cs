using System;
using System.Reflection;

namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class AbstractMappingAttribute : Attribute
    {
        public abstract bool Mapping(TAAction ac, string key, Type t, out object result);
    }
}
