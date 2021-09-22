using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreJsonFormParser : ITANetCoreFormParser
    {
        public ITAFormCollection ParseForm(HttpRequest request, LinkedListNode<ITANetCoreFormParser> next)
        {
            if (request.ContentType == null)
            {
                if (next == null)
                {
                    return null;
                }
                return next.Value.ParseForm(request, next.Next);
            }
            string encodingstr = "UTF-8";
            string ctstr = "";
            string[] tarr = request.ContentType.Split(";");

            if (tarr.Length > 0)
            {
                ctstr = tarr[0].ToLower();
                for (int i = 1; i < tarr.Length; i++)
                {
                    string tmpstr = tarr[i].ToLower();
                    int tjidx = tmpstr.IndexOf("=");
                    string tmpkey = tmpstr.Substring(0, tjidx).Trim();
                    string tmpval = tmpstr.Substring(tjidx + 1, tmpstr.Length - tjidx - 1).Trim();
                    switch (tmpkey)
                    {
                        case "charset":
                            encodingstr = tmpval;
                            break;
                    }
                }
            }
            else
            {
                ctstr = request.ContentType.ToLower();
            }
            if (ctstr.Trim() == "application/json")
            {
                StreamReader sr = new StreamReader(request.Body, Encoding.GetEncoding(encodingstr));
                string bodystr = sr.ReadToEnd();
                Dictionary<string, object> dic = MyAccess.Json.Json.DecodeType<Dictionary<string, object>>(bodystr);
                return new TANetCoreHttpFormDictionary(dic, null);
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
