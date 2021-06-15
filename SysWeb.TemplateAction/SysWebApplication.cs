using System;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    /// <summary>
    /// 完全懒汉单例
    /// </summary>
    public class SysWebApplication
    {
        private SysWebApplication() { }
        private class Nested
        {
            // 显式静态构造告诉C＃编译器未标记类型BeforeFieldInit
            // 保证在调用Nested静态类时才进行实例初始化
            static Nested(){}
            internal static readonly TAApplication Instance = new TAApplication();
        }
        public static TAApplication Application
        {
            get
            {
                return Nested.Instance.Load();
            }
        }
    }
}
