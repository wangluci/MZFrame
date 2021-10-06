using System;
using System.Reflection;

namespace TemplateAction.Core
{
    public class QueryMappingAttribute : AbstractMappingAttribute
    {
        public override bool Mapping(TAAction ac, string key, Type t, out object result)
        {
            return DefatulParamMapping.TAObjectMapping(ac.Context.Request.Query, key, t, out result);
        }
    }
}
