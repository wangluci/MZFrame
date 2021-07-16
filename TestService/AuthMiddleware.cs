﻿using System;
using TemplateAction.Core;

namespace TestService
{
    public class AuthMiddleware : IFilterMiddleware
    {
        public object Excute(TARequestHandle request, FilterMiddlewareNode next)
        {
            if (request.NameSpace == "TestService" && request.Controller == "Home" && request.Action == "Test")
            {
                //需要身份认证
                request.Context.Response.Write("带有身份认证\n");
                return next.Excute(request);
            }
            else
            {
                //不需要认证
                return next.Excute(request);
            }
        }
    }
}
