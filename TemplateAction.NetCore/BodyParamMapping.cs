using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public delegate object? DecodeJson(string json, Type t);
    public delegate object? DecodeXml(string xml, Type t);
    public class BodyParamMapping : IParamMapping
    {
        private DecodeJson _jsonFun;
        private DecodeXml _xmlFun;
        public BodyParamMapping(DecodeJson json, DecodeXml xml)
        {
            _jsonFun = json;
            _xmlFun = xml;
        }
        public BodyParamMapping(DecodeJson json) : this(json, DefaultXml) { }
        public BodyParamMapping(DecodeXml xml) : this(DefaultJson, xml) { }
        public BodyParamMapping() : this(DefaultJson, DefaultXml) { }

        internal static DecodeJson DefaultJson = (json, t) =>
        {
            return JsonSerializer.Deserialize(json, t);
        };
        internal static DecodeXml DefaultXml = (xml, t) =>
        {
            StringReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(t);
            object rt = serializer.Deserialize(reader);
            reader.Close();
            return rt;
        };
        internal static bool BodyMapping(DecodeJson json, DecodeXml xml, TAAction ac, string key, Type t, out object result)
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
                                result = json(sr.ReadToEnd(), t);
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
                                result = xml(sr.ReadToEnd(), t);
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
            if (BodyMapping(_jsonFun, _xmlFun, ac, key, t, out result))
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
