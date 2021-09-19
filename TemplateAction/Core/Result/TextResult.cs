using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 文本结果
    /// </summary>
    public class TextResult : IResult
    {
        private ITAContext _context;
        private string mContentType;
        private string mContent;
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }


        public TextResult(ITAContext context, string content)
        {
            mContentType = "text/plain";
            _context = context;
            mContent = content;
        }
        public override string ToString()
        {
            return mContent;
        }
        public void Output()
        {
            _context.Response.ContentType = mContentType;
            _context.Response.Write(mContent);
        }
    }
}
