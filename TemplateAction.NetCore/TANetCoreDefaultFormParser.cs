using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreDefaultFormParser : ITANetCoreFormParser
    {
        public ITAFormCollection ParseForm(HttpRequest request, LinkedListNode<ITANetCoreFormParser> next)
        {
            if (request.HasFormContentType)
            {
                IFormCollection fc = request.ReadFormAsync().GetAwaiter().GetResult();
                int filecount = request.Form.Files.Count;
                TANetCoreHttpFile[] requestFiles = new TANetCoreHttpFile[filecount];
                for (int i = 0; i < filecount; i++)
                {
                    requestFiles[i] = new TANetCoreHttpFile(request.Form.Files[i]);
                }
                return new TANetCoreHttpFormCollection(fc, requestFiles);
            }
            else
            {
                if (next == null)
                {
                    return null;
                }
                return next.Value.ParseForm(request, next.Next);
            }
        }
    }
}
