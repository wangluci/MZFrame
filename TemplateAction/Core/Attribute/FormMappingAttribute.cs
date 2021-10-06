using System;
using System.Reflection;

namespace TemplateAction.Core
{
    /// <summary>
    /// 绑定form
    /// </summary>
    public class FormMappingAttribute : AbstractMappingAttribute
    {
        public override bool Mapping(TAAction ac, string key, Type t, out object result)
        {
            return DefatulParamMapping.TAObjectMapping(ac.Context.Request.Form, key, t, out result);
        }
    }
}
