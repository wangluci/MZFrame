using MyNet.Middleware.Http;
using System;
using System.Collections;
using TemplateAction.Common;
using TemplateAction.Core;
namespace MyNet.TemplateAction
{
    public class MyNetObjectCollection : ITAObjectCollection
    {
        private HttpNameValueCollection _hnvc;
        public MyNetObjectCollection(HttpNameValueCollection hnvc)
        {
            _hnvc = hnvc;
        }
        public object this[string key]
        {
            get
            {
                return _hnvc[key];
            }
        }

        public int Count
        {
            get { return _hnvc.Count; }
        }


        public IEnumerator GetEnumerator()
        {
            foreach (string key in _hnvc.Keys)
            {
                yield return new TAObject(key, _hnvc[key]);
            }
        }


        public bool TryGet(string key, out object result)
        {
            result = _hnvc.Get(key);
            return result == null ? false : true;
        }
    }
}
