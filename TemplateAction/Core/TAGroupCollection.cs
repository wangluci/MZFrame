using System;
using System.Collections;
using System.Collections.Generic;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    public class TAGroupCollection : ITAObjectCollection
    {
        ITAObjectCollection _one;
        ITAObjectCollection _two;
        public TAGroupCollection(ITAObjectCollection one, ITAObjectCollection two)
        {
            _one = one;
            _two = two;
        }

        public object this[string key]
        {
            get
            {
                object rt = _one[key];
                if (rt != null)
                {
                    return rt;
                }
                rt = _two[key];
                if (rt != null)
                {
                    return rt;
                }
                return rt;
            }
        }

        public int Count
        {
            get { return _one.Count + _two.Count; }
        }

        public T Cast<T>(string key, T def)
        {
            object result;
            if (TryGet(key, out result))
            {
                return TAConverter.Cast(result, def);
            }
            else
            {
                return def;
            }
        }


        public bool TryConvert(string key, Type targetType, out object result)
        {
            if (TryGet(key, out result))
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

        public bool TryGet(string key, out object result)
        {
            if (_one.TryGet(key, out result))
            {
                return true;
            }
            else
            {
                return _two.TryGet(key, out result);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (TAObject obj in _one)
            {
                yield return obj;
            }
            foreach (TAObject obj in _two)
            {
                yield return obj;
            }
        }
    }
}
