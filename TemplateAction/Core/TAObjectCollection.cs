
using System;
using System.Collections;
using System.Collections.Generic;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    public class TAObjectCollection : ITAObjectCollection
    {
        private Dictionary<string, object> _values;

        public int Count
        {
            get { return _values.Count; }
        }

        public TAObjectCollection()
        {
            _values = new Dictionary<string, object>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<string, object> kvp in _values)
            {
                yield return new TAObject(kvp.Key, kvp.Value);
            }
        }
        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }
        public void Add(string key, object value)
        {
            _values[key] = value;
        }
        public object this[string key]
        {
            get
            {
                object rt;
                if (_values.TryGetValue(key, out rt))
                {
                    return rt;
                }
                return null;
            }
        }
        public bool TryGet(string key, out object result)
        {
            return _values.TryGetValue(key, out result);
        }
        public bool TryConvert(string key, Type targetType, out object result)
        {
            if (_values.TryGetValue(key, out result))
            {
                if (TAConverter.Instance.TryConvert(result, targetType, out result))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public T Cast<T>(string key, T def)
        {
            object result;
            if (_values.TryGetValue(key, out result))
            {
                return TAConverter.Cast(result, def);
            }
            else
            {
                return def;
            }
        }
    }
}
