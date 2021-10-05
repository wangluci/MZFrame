using System;
using System.Collections;
using System.Reflection;
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

        public bool Mapping(ParameterInfo pi, out object result)
        {
            ITARequest req = _ac.Context.Request;
            if (req.Query != null)
            {
                if (req.Query.Mapping(pi, out result))
                {
                    return true;
                }
            }
            if (req.Form != null)
            {
                if (req.Form.Mapping(pi, out result))
                {
                    return true;
                }
            }
            if (_ac.ExtParams != null)
            {
                return _ac.ExtParams.Mapping(pi, out result);
            }
            else
            {
                result = null;
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
                yield return req.Query.GetEnumerator();
            }
            if (req.Form != null)
            {
                yield return req.Form.GetEnumerator();
            }
            if (_ac.ExtParams != null)
            {
                yield return _ac.ExtParams.GetEnumerator();
            }
        }
    }
}
