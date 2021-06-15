using System;

namespace MyAccess.Json
{
    public interface IDateTimeFormat
    {
        string Format(DateTime val);
    }
    public class DefaultDateTimeFormat : IDateTimeFormat
    {
        public string _template;
        public DefaultDateTimeFormat(string template)
        {
            _template = template;
        }
        public string Format(DateTime val)
        {
            return val.ToString(_template);
        }
    }
}
