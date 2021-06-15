using System;
using System.Reflection;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 控制器树节点
    /// </summary>
    public class ControllerNode : Node
    {
        private Type mType;
        public Type ControllerType
        {
            get { return mType; }
        }
        public ControllerNode(PluginObject plugin, Type type)
        {
            mKey = type.Name.ToLower();
            object[] attrs = type.GetCustomAttributes(typeof(DesAttribute), false);
            mType = type;
            foreach (object attrobj in attrs)
            {
                DesAttribute ad = attrobj as DesAttribute;
                if (ad != null)
                {
                    mDescript = ad.Des;
                }
            }
            InitActions(plugin);
        }
        /// <summary>
        /// 初始化动作节点
        /// </summary>
        private void InitActions(PluginObject plugin)
        {
            MethodInfo[] methodArray = mType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methodArray)
            {
                //判断方法返回值是否继承IResult
                if (!typeof(IResult).IsAssignableFrom(method.ReturnType))
                {
                    continue;
                }
                if (method.IsVirtual || method.IsStatic || method.DeclaringType.Name.Equals("Object"))
                {
                    continue;
                }
                ActionNode an = new ActionNode(plugin, mKey, method);
                AddChildNode(an.Key, an);
            }
        }
    }
}
