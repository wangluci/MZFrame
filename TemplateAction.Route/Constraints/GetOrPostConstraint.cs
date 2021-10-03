using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// 只接收Get或Post请求
    /// </summary>
    public class GetOrPostConstraint : IRouteConstraint
    {
        public bool Match(ITAContext context, IRouter route, string routeKey, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            string httpmethod = context.Request.HttpMethod.ToLower();
            switch (httpmethod)
            {
                case "get":
                    return true ;
                case "post":
                    return true;
                default:
                    return false;
            }
        }
    }
}
