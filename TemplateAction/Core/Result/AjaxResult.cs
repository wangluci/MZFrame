using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// ajax结果，进行ajax操作时使用
    /// </summary>
    public class AjaxResult : IResult
    {
        protected int mCode;
        protected string mMessage;
        protected string mData;
        protected ITAContext mContext;
        public string Data
        {
            get { return mData; }
        }

        /// <summary>
        /// Ajax请求结果返回
        /// </summary>
        /// <param name="isErr">错误代码</param>
        /// <param name="message">提示消息</param>
        /// <param name="jsondata">返回json字符串</param>
        public AjaxResult(ITAContext context, int code, string message, string jsondata = null)
        {
            mCode = code;
            mMessage = message;
            mData = jsondata;
            mContext = context;
        }
    
        public void Output()
        {
            mContext.Response.ContentType = "application/json";
            if (mData == null)
            {
                mContext.Response.Write("{\"Code\":" + mCode + ",\"Message\":\"" + Common.TAUtility.AllFilter(mMessage) + "\"}");
            }
            else
            {
                mContext.Response.Write("{\"Code\":" + mCode + ",\"Message\":\"" + Common.TAUtility.AllFilter(mMessage) + "\",\"Data\":" + mData + "}");
            }
        }
    }
}
