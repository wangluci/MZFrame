using System;
using System.Reflection;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    public interface IController
    {
        IRequestHandle RequestHandle { get; }
        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <param name="controller"></param>
        void Init(IRequestHandle handle);
        /// <summary>
        /// 执行动作
        /// </summary>
        object CallAction(MethodInfo method, object[] parameters);
        /// <summary>
        /// 异常时执行
        /// </summary>
        /// <param name="ex"></param>
        IResult Exception(Exception ex);

    }
}
