using System.Collections;
using System.Collections.Generic;
using TemplateAction.Core;
namespace TemplateAction.NetCore
{
    public class TANetCoreHttpFormDictionary : ITAFormCollection
    {
        private IDictionary<string, object> _form;
        private IRequestFile[] _files;
        public TANetCoreHttpFormDictionary(IDictionary<string, object> collection, IRequestFile[] files)
        {
            _form = collection;
        }

        public object this[string key]
        {
            get
            {
                object sv;
                if (_form.TryGetValue(key, out sv))
                {
                    return sv;
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
        public IRequestFile[] Files
        {
            get { return _files; }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string key in _form.Keys)
            {
                yield return new TAObject(key, _form[key]);
            }
        }

        public bool TryGet(string key, out object result)
        {
            object val;
            if (_form.TryGetValue(key, out val))
            {
                result = val;
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
