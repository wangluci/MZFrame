using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpContext : ITAContext
    {
        private TANetCoreHttpApplication _app;
        private ITASession _tasession;
        private ITARequest _request;
        private ITAResponse _response;
        private HttpContext _context;
        private IDictionary _dicItems;
        public TANetCoreHttpContext(TANetCoreHttpApplication app, HttpContext context)
        {
            _app = app;
            ISessionFeature sessionFeature = context.Features.Get<ISessionFeature>();
            if (sessionFeature != null)
            {
                //创建Session
                _tasession = new TANetCoreHttpSession(context.Session);
            }
            _context = context;
            _request = new TANetCoreHttpRequest(context);
            _response = new TANetCoreHttpResponse(context);
            _dicItems = new Hashtable();
        }
        public TASiteApplication Application
        {
            get { return _app; }
        }

        public string Version
        {
            get { return string.Empty; }
        }

        public ITARequest Request { get { return _request; } }

        public ITAResponse Response { get { return _response; } }

        public IDictionary Items
        {
            get { return _dicItems; }
        }

        public ITASession Session
        {
            get { return _tasession; }
        }

        public string MapPath(string path)
        {
            if (path == null) return string.Empty;
            int ss = 0;
            if (path.Length > 0)
            {
                if (path[0] == '~')
                {
                    ss++;
                }
            }
            if (ss < path.Length)
            {
                if (path[ss] == '/')
                {
                    ss++;
                }
            }

            if (ss > 0)
            {
                path = path.Substring(ss);
            }
            //windows系统
            if (Path.DirectorySeparatorChar.Equals('\\'))
            {
                path = path.Replace("/", "\\");
            }
            IHostingEnvironment env = _context.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
            return Path.Combine(env.WebRootPath, path);
        }

        public string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding).Replace("%2b", "+");
        }

        public ITACookie CreateCookie(string name)
        {
            return new TANetCoreHttpCookie(_context, name);
        }

        public ITACookie CreateCookie(string name, string encodekey)
        {
            return new TANetCoreHttpCookie(_context, name, encodekey);
        }

        public bool ExistCookie(string name)
        {
            return _context.Request.Cookies.ContainsKey(name);
        }
        public void SaveCookie(ITACookie cookie)
        {
            ((TANetCoreHttpCookie)cookie).SaveCookie();
        }

    }
}
