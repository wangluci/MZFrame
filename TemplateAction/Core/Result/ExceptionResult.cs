using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 异常结果
    /// </summary>
    public class ExceptionResult : AjaxResult
    {
        public ExceptionResult(ITAContext context, Exception ex) : base(context, -99999, ex.Message, AjaxResult.JsonData(ex.StackTrace))
        {
        }
    }
}
