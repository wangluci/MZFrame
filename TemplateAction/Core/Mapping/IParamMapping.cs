using System.Reflection;
namespace TemplateAction.Core
{
    public interface IParamMapping
    {
        object Mapping(ITAObjectCollection param, ParameterInfo pi);
    }
}
