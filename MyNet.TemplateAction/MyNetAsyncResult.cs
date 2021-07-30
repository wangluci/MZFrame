using TemplateAction.Label;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MyNetAsyncResult : ITAAsyncResult
    {
        private HttpContext _context;
        public MyNetAsyncResult(HttpContext context)
        {
            _context = context;
        }

        public void Completed(IResult data)
        {
            if (data != null)
            {
                data.Output();
            }
            _context.RequestFinish();
        }

    }
}
