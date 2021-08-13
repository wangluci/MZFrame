using System;
using System.Reflection;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    public interface IController
    {
        ITAAction RequestHandle { get; }
        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <param name="controller"></param>
        void Init(ITAAction handle);
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
