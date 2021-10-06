using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IParamMapping
    {
        bool Mapping(LinkedListNode<IParamMapping> next,TAAction ac, string key, Type t, out object result);
    }
}
