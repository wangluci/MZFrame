using MyNet.Middleware.Http;
using System;
using TemplateAction.Core;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MyNet.Handlers;

namespace MyNet.TemplateAction
{
    public class HttpContext : ITAContext
    {
        private ITARequest _request;
        public ITARequest Request { get { return _request; } }
        private ITAResponse _response;
        public ITAResponse Response { get { return _response; } }
        /// <summary>
        /// 当前版本信息
        /// </summary>
        public static string HttpVersion = Common.Utility.VERSION;
        private HttpSite _site;
        private ITASession _session;
        private IDictionary _dicItems;
        private Dictionary<string, IController> _controllers;
        private IContext _context;
        public IContext Context
        {
            get { return _context; }
        }
        public HttpContext(IContext context, HttpRequest request, HttpResponse response, HttpSite site)
        {
            _context = context;
            _controllers = new Dictionary<string, IController>();
            _request = new MyNetRequest(request);
            _response = new MyNetResponse(_context, response);
            _site = site;
            _dicItems = new Hashtable();
        }
        internal void CreateSession()
        {
            _session = _site.GetSessionManager().CreateSession(this);
        }

        public TASiteApplication Application
        {
            get
            {
                return _site.Application;
            }
        }

        public ITASession Session
        {
            get { return _session; }
        }

        public string Version
        {
            get
            {
                return HttpVersion;
            }
        }


        public string MapPath(string path)
        {
            return _site.MapPath(path);
        }



        public string UrlDecode(string str, Encoding encoding)
        {
            return Common.Utility.UrlDecode(str, encoding);
        }

        public IDictionary Items
        {
            get
            {
                return _dicItems;
            }
        }
        public void RequestFinish()
        {
            ((MyNetResponse)_response).End();
        }

        public ITACookie CreateCookie(string name)
        {
            return ((MyNetRequest)_request).CreateCookie(name);
        }

        public ITACookie CreateCookie(string name, string encodekey)
        {
            return ((MyNetRequest)_request).CreateCookie(name, encodekey);
        }

        public bool ExistCookie(string name)
        {
            return ((MyNetRequest)_request).ExistCookie(name);
        }

        public void SaveCookie(ITACookie cookie)
        {
            ((MyNetResponse)_response).SaveCookie(cookie);
        }
    }
}
