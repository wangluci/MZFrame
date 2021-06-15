
using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class V404Result : IResult
    {
        ITAContext _content;
        public V404Result(ITAContext context)
        {
            _content = context;
        }
        public void Output()
        {
            _content.Response.StatusCode = 404;
            _content.Response.StatusDescription = "Not Found";
        }
    }
}
