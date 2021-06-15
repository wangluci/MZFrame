using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 动作关联
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AboutAttribute : Attribute
    {
        /// <summary>
        /// 关联模块
        /// </summary>
        private string mAbountModule;
        /// <summary>
        /// 关联动作
        /// </summary>
        private string mAbountAction;

        public AboutAttribute(string module,string action)
        {
            mAbountModule = module.ToLower();
            mAbountAction = action.ToLower();
        }
        public AboutAttribute(string action)
        {
            mAbountModule = string.Empty;
            mAbountAction = action.ToLower();
        }
        public string AbountModule { get { return mAbountModule; } }
        public string AbountAction { get { return mAbountAction; } }
    }
}
