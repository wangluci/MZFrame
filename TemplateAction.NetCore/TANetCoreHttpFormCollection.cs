using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpFormCollection : ITAObjectCollection
    {
        private IFormCollection _form;
        public TANetCoreHttpFormCollection(IFormCollection collection)
        {
            _form = collection;
        }

        public object this[string key]
        {
            get
            {
                StringValues sv;
                if (_form.TryGetValue(key, out sv))
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
            get { return _form.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string key in _form.Keys)
            {
                yield return new TAObject(key, _form[key].ToString());
            }
        }

        public bool TryGet(string key, out object result)
        {
            StringValues val;
            if (_form.TryGetValue(key, out val))
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
