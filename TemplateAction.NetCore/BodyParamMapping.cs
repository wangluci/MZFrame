using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class BodyParamMapping : IParamMapping
    {
        internal static bool BodyMapping(TAAction ac, string key, Type t, out object result)
        {
            ITARequest req = ac.Context.Request;
            string contenttype = req.Header["content-type"];
            if (contenttype != null)
            {
                string encodingstr = "UTF-8";
                string ctstr = "";
                string[] tarr = contenttype.Split(";");

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
                    ctstr = contenttype.ToLower();
                }
                ctstr = ctstr.Trim();
                switch (ctstr)
                {
                    case "application/json":
                        try
                        {
                            StreamReader sr = new StreamReader(req.InputStream, Encoding.GetEncoding(encodingstr));
                            if (typeof(string).Equals(t))
                            {
                                result = sr.ReadToEnd();
                            }
                            else
                            {
                                result = JsonSerializer.Deserialize(sr.ReadToEnd(), t);
                            }
                            return true;
                        }
                        catch { }
                        break;
                    case "application/xml":
                        try
                        {
                            StreamReader sr = new StreamReader(req.InputStream, Encoding.GetEncoding(encodingstr));
                            if (typeof(string).Equals(t))
                            {
                                result = sr.ReadToEnd();
                            }
                            else
                            {
                                StringReader reader = new StringReader(sr.ReadToEnd());
                                XmlSerializer serializer = new XmlSerializer(t);
                                result = serializer.Deserialize(req.InputStream);
                                reader.Close();
                            }
                            return true;
                        }
                        catch { }
                        break;
                }
            }
            result = null;
            return false;
        }
        public bool Mapping(LinkedListNode<IParamMapping> next, TAAction ac, string key, Type t, out object result)
        {
            if (BodyMapping(ac, key, t, out result))
            {
                return true;
            }
            if (next == null)
            {
                return false;
            }
            return next.Value.Mapping(next.Next, ac, key, t, out result);
        }
    }
}
