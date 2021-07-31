using System;
using System.Reflection;
using System.Threading.Tasks;
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
        private string mPluginName;
        /// <summary>
        /// 所属模块名
        /// </summary>
        public string PluginName
        {
            get { return mPluginName; }
        }
        public ControllerNode(PluginObject plugin, Type type)
        {
            mKey = type.Name;
            mPluginName = plugin.Name;
            DesAttribute ad = (DesAttribute)type.GetCustomAttribute(typeof(DesAttribute));
            if (ad != null)
            {
                mDescript = ad.Des;
            }
            mType = type;
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
                    bool iacontinue = true;
                    if (method.ReturnType == typeof(Task))
                    {
                        iacontinue = false;
             
                    }
                    else if (method.ReturnType.IsGenericType)
                    {
                        if(method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            if (method.ReturnType.GenericTypeArguments.Length > 0)
                            {
                                Type gtype = method.ReturnType.GenericTypeArguments[0];
                                if (typeof(IResult).IsAssignableFrom(gtype))
                                {
                                    iacontinue = false;
                                }
                            }
                        }
                    }
                    if (iacontinue)
                    {
                        continue;
                    }
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
