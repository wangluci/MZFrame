using System;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    /// <summary>
    /// 异常结果
    /// </summary>
    public class ExceptionResult : IResult
    {
        protected ITAContext mContext;
        private Exception mEx;
        public ExceptionResult(ITAContext context, Exception ex)
        {
            mContext = context;
            mEx = ex;
        }

        public void Output()
        {
            mContext.Response.ContentType = "application/json";
            mContext.Response.Write("{\"Code\":-20000,\"Message\":\"" + Common.TAUtility.Unicode(mEx.Message.Replace("\"", "")) + "\",\"Data\":\"" + Common.TAUtility.Unicode(mEx.StackTrace.Replace("\"", "")) + "\"}");
        }
    }
}
