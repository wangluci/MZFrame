using System;
using TemplateAction.Core;

namespace AuthService
{
    public static class AuthExtension
    {
        public static T GetItem<T>(this ITAContext context,string key)
        {
            return (T)context.Items[key];
        }
        public static string GetTerminal(this ITAContext context)
        {
            string agent = context.Request.UserAgent.ToLower();
            if (agent.Contains("micromessenger"))
            {
                return "WeiXin:" + context.Request.ClientIP;
            }
            else if (agent.Contains("iphone") || agent.Contains("ipod") || agent.Contains("ipad"))
            {
                return "ios:" + context.Request.ClientIP;
            }
            else if (agent.Contains("android"))
            {
                return "android:" + context.Request.ClientIP;
            }
            else
            {
                return "pc:" + context.Request.ClientIP;
            }
        }
    }
}
