using System;
using System.Reflection;
namespace TemplateAction.Core
{
    public class ValueMapping : IParamMapping
    {
        public object Mapping(ITAObjectCollection param, ParameterInfo pi)
        {
            Type destType = pi.ParameterType;
            if (destType == typeof(string))
            {
                object result;
                if (param.TryGet(pi.Name,out result))
                {
                    return result;
                }
                else
                {
                    if (pi.DefaultValue != DBNull.Value)
                    {
                        return pi.DefaultValue;
                    }
                }
                return null;
            }
            else
            {
                object tvalobj;
                if (param.TryConvert(pi.Name, destType, out tvalobj))
                {
                    return tvalobj;
                }
                else
                {
                    if (pi.DefaultValue != DBNull.Value)
                    {
                        return pi.DefaultValue;
                    }
                }
                return null;
            }
        }
    }
}
