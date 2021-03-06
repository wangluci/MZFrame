
using System;
using System.Collections;
using System.Collections.Specialized;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class SysWebObjectCollection : ITAObjectCollection
    {
        private NameValueCollection _nvc;
        public SysWebObjectCollection(NameValueCollection nvc)
        {
            _nvc = nvc;
        }
        public object this[string key]
        {
            get
            {
                return _nvc[key];
            }
        }

        public int Count
        {
            get { return _nvc.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            yield return _nvc.GetEnumerator();
        }

        public bool TryGet(string key, out object result)
        {
            result = _nvc.Get(key);
            return result == null ? false : true;
        }
    }
}
