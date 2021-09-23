using System;
using TemplateAction.Core;

namespace AuthService
{
    public class FatherController : TABaseController
    {
        protected string GetTerminal()
        {
            string agent = Context.Request.UserAgent.ToLower();
            if (agent.Contains("micromessenger"))
            {
                return "WeiXin:" + Context.Request.ClientIP;
            }
            else if (agent.Contains("iphone") || agent.Contains("ipod") || agent.Contains("ipad"))
            {
                return "ios:" + Context.Request.ClientIP;
            }
            else if (agent.Contains("android"))
            {
                return "android:" + Context.Request.ClientIP;
            }
            else
            {
                return "pc:" + Context.Request.ClientIP;
            }
        }
    }
}
