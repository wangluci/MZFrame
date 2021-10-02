using System;
using System.Collections;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    public class TAGroupCollection : ITAObjectCollection
    {
        private TAAction _ac;
        public TAGroupCollection(TAAction ac)
        {
            _ac = ac;
        }

        public object this[string key]
        {
            get
            {
                object rt = null;
                ITARequest req = _ac.Context.Request;
                if (req.Query != null)
                {
                    rt = req.Query[key];
                    if (rt != null)
                    {
                        return rt;
                    }
                }
                if (req.Form != null)
                {
                    rt = req.Form[key];
                    if (rt != null)
                    {
                        return rt;
                    }
                }

                if (_ac.ExtParams != null)
                {
                    return _ac.ExtParams[key];
                }
                return rt;
            }
        }

        public int Count
        {
            get
            {
                ITARequest req = _ac.Context.Request;
                int totalcount = 0;
                if (req.Query != null)
                {
                    totalcount += req.Query.Count;
                }
                if (req.Form != null)
                {
                    totalcount += req.Form.Count;
                }
                if (_ac.ExtParams != null)
                {
                    totalcount += _ac.ExtParams.Count;
                }
                return totalcount;
            }
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
            ITARequest req = _ac.Context.Request;
            if (req.Query != null)
            {
                if (req.Query.TryGet(key, out result))
                {
                    return true;
                }
            }
            if (req.Form != null)
            {
                if (req.Form.TryGet(key, out result))
                {
                    return true;
                }
            }
            if (_ac.ExtParams != null)
            {
                return _ac.ExtParams.TryGet(key, out result);
            }
            else
            {
                result = null;
                return false;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            ITARequest req = _ac.Context.Request;
            if (req.Query != null)
            {
                foreach (TAObject obj in req.Query)
                {
                    yield return obj;
                }
            }
            if (req.Form != null)
            {
                foreach (TAObject obj in req.Form)
                {
                    yield return obj;
                }
            }
          
            if (_ac.ExtParams != null)
            {
                foreach (TAObject obj in _ac.ExtParams)
                {
                    yield return obj;
                }
            }
        }
    }
}
