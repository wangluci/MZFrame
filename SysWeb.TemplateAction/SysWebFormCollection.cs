
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using TemplateAction.Core;
namespace SysWeb.TemplateAction
{
    public class SysWebFormCollection : ITAFormCollection
    {
        private NameValueCollection _nvc;
        private IRequestFile[] _files;
        public SysWebFormCollection(NameValueCollection nvc, HttpFileCollection files)
        {
            _nvc = nvc;
            _files = new IRequestFile[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                _files[i] = new SysWebRequestFile(files[i]);
            }
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

        public IRequestFile[] Files
        {
            get { return _files; }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string key in _nvc.Keys)
            {
                yield return new TAObject(key, _nvc[key]);
            }
        }

        public bool TryGet(string key, out object result)
        {
            result = _nvc.Get(key);
            return result == null ? false : true;
        }
    }
}
