using System;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    /// <summary>
    /// 异常结果
    /// </summary>
    public class ExceptionResult : IResult
    {
        protected IRequestHandle mHandle;
        private Exception mEx;
        public ExceptionResult(IRequestHandle handle, Exception ex)
        {
            mHandle = handle;
            mEx = ex;
        }

        public void Output()
        {
            mHandle.Context.Response.ContentType = "application/json";
            mHandle.Context.Response.Write("{\"Code\":-20000,\"Message\":\"" + Common.TAUtility.Unicode(mEx.Message.Replace("\"", "")) + "\",\"Data\":\"" + Common.TAUtility.Unicode(mEx.StackTrace.Replace("\"", "")) + "\"}");
        }
    }
}
