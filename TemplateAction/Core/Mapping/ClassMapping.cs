using System;
using System.Reflection;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class ClassMapping : IParamMapping
    {
        public object Mapping(ITAObjectCollection param, ParameterInfo pi)
        {
            ConstructorInfo ct1 = pi.ParameterType.GetConstructor(Type.EmptyTypes);
            if (ct1 == null) return null;
            object ttobj = ct1.Invoke(null);
            if (ttobj == null) return null;

            PropertyInfo[] myallProinfos = pi.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (PropertyInfo myProInfo in myallProinfos)
            {
                if (myProInfo.GetIndexParameters().Length != 0) return null;
                object tvalobj;
                if (param.TryConvert(myProInfo.Name, myProInfo.PropertyType, out tvalobj))
                {
                    myProInfo.SetValue(ttobj, tvalobj, null);
                }
            }

            return ttobj;
        }
    }
}
