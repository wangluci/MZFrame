using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TextResult : IResult
    {
        private ITAAction mHandle;
        private string mContentType;
        private string mContent;
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }
        /// <summary>
        /// Ajax请求结果返回
        /// </summary>
        /// <param name="isErr">错误代码</param>
        /// <param name="message">提示消息</param>
        /// <param name="jsondata">返回json字符串</param>
        public TextResult(ITAAction handle, string content)
        {
            mContentType = "text/plain";
            mHandle = handle;
            mContent = content;
        }
        public override string ToString()
        {
            return mContent;
        }
        public void Output()
        {
            mHandle.Context.Response.ContentType = mContentType;
            mHandle.Context.Response.Write(mContent);
        }
    }
}
