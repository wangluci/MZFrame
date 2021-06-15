﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpQueryCollection : ITAObjectCollection
    {
        private IQueryCollection _query;
        public TANetCoreHttpQueryCollection(IQueryCollection collection)
        {
            _query = collection;
        }
        public object this[string key]
        {
            get
            {
                StringValues sv;
                if(_query.TryGetValue(key,out sv))
                {
                    return sv.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get { return _query.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string key in _query.Keys)
            {
                yield return new TAObject(key, _query[key].ToString());
            }
        }

        public bool TryGet(string key, out object result)
        {
            StringValues val;
            if(_query.TryGetValue(key,out val))
            {
                result = val.ToString();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
