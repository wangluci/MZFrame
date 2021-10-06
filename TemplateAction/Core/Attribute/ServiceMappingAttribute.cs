using System;

namespace TemplateAction.Core
{
    public class ServiceMappingAttribute : AbstractMappingAttribute
    {
        public override bool Mapping(TAAction ac, string key, Type t, out object result)
        {
            result = ac.Context.Application.ServiceProvider.GetService(t);
            if (result == null)
            {
                return false;
            }
            return true;
        }
    }
}
