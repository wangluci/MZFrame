using System;

namespace TemplateAction.Core
{
    public class TAObject
    {
        private string _key;
        private object _value;
        public string Key { get { return _key; } }
        public object Value { get { return _value; } }
           
        public TAObject(string key,object value)
        {
            _key = key;
            _value = value;
        }
    }
}
