using Microsoft.AspNetCore.Http;
using System;
using TemplateAction.Core;
using System.Collections.Generic;
namespace TemplateAction.NetCore
{
    /// <summary>
    /// 根据ContentType解释成Form
    /// </summary>
    public interface ITANetCoreFormParser
    {
        ITAFormCollection ParseForm(HttpRequest request, LinkedListNode<ITANetCoreFormParser> next);
    }
}
