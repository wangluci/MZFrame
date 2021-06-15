using MyNet.Handlers;
using MyNet.Middleware.Http;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class ActionEvent
    {
        private IContext _context;
        public IContext Context
        {
            get { return _context; }
        }
        private HttpRequest _request;
        public HttpRequest Request
        {
            get { return _request; }
        }
        public ActionEvent(IContext context, HttpRequest request)
        {
            _context = context;
            _request = request;
        }
    }
}
