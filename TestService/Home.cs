using ShareService;
using System;

using TemplateAction.Core;

namespace TestService
{
    public class Home : TABaseController
    {
        private ShareInstance _share;
        public Home(ShareInstance share)
        {
            _share = share;
        }
        public ViewResult Index()
        {
            return View();
        }
        /// <summary>
        /// Filter中间件测试
        /// </summary>
        /// <returns></returns>
        public TextResult Test()
        {
            return Content("身份认证例子");
        }
        /// <summary>
        /// 共享模块测试
        /// </summary>
        /// <returns></returns>
        public TextResult Share()
        {
            return Content(_share.Convert2Json());
        }
    }
}
