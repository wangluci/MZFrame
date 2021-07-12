using System;

using TemplateAction.Core;

namespace TestService
{
    public class Home : TABaseController
    {
        public ViewResult Index()
        {
            return View();
        }
        public TextResult Test()
        {
            return Content("身份认证例子");
        }
    }
}
