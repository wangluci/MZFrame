using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateAction.Core
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public abstract class ActionNodeAttribute : Attribute
    {
        public abstract void ConfigAction(ActionNode node);
    }
}
