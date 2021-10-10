using System;

using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class BodyMappingAttribute : AbstractMappingAttribute
    {
        private DecodeJson _json;
        private DecodeXml _xml;
        /// <summary>
        /// 默认使用系统json,xml
        /// </summary>
        public BodyMappingAttribute()
        {
            _json = CreateJsonDecode();
            _xml = CreateXmlDecode();
        }
        protected virtual DecodeJson CreateJsonDecode()
        {
            return BodyParamMapping.DefaultJson;
        }
        protected virtual DecodeXml CreateXmlDecode()
        {
            return BodyParamMapping.DefaultXml;
        }
        public override bool Mapping(TAAction ac, string key, Type t, out object result)
        {
            return BodyParamMapping.BodyMapping(_json, _xml, ac, key, t, out result);
        }
    }
}
