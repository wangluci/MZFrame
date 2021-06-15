using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class MappingFactory
    {
        private static MappingFactory MappingInstance = new MappingFactory();

        private ArraryMapping mArraryMapping;
        private ClassMapping mClassMapping;
        private ValueMapping mValueMapping;
        private MappingFactory()
        {
            mArraryMapping = new ArraryMapping();
            mClassMapping = new ClassMapping();
            mValueMapping = new ValueMapping();
        }

        public static object Mapping(ITAObjectCollection param, ParameterInfo pi)
        {
            if (pi.ParameterType.IsClass && pi.ParameterType != typeof(string))
            {
                if (pi.ParameterType.IsArray)
                {
                    return MappingInstance.mArraryMapping.Mapping(param, pi);
                }
                else
                {
                    return MappingInstance.mClassMapping.Mapping(param, pi);
                }
            }
            else
            {
                return MappingInstance.mValueMapping.Mapping(param, pi);
            }
        }
    }
}
