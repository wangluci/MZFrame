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
    }
}
