using System;

using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class BodyMappingAttribute : AbstractMappingAttribute
    {
        public override bool Mapping(TAAction ac, string key, Type t, out object result)
        {
            return BodyParamMapping.BodyMapping(ac, key, t, out result);
        }
    }
}
