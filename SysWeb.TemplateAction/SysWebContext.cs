using System;
using System.Collections;
using System.Text;
using System.Web;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class SysWebContext : ITAContext
    {
        ITASession _session;
        HttpContext _httpContext;
      
        ITAResponse _response;
        ITARequest _request;
        public SysWebContext(HttpContext context)
        {
            _httpContext = context;
            _request = new SysWebRequest(_httpContext.Request);
            _response = new SysWebResponse(_httpContext.Response);
        }

        internal void CreateSession()
        {
            _session = MySessionManager.Instance().CreateSession(this);
        }


        public ITASession Session
        {
            get { return _session; }
        }

        public string Version
        {
            get
            {
                return _httpContext.Request.ServerVariables["SERVER_SOFTWARE"];
            }
        }


        public string MapPath(string path)
        {
            return _httpContext.Server.MapPath(path); ;
        }


        public TAApplication Application
        {
            get
            {
                return SysWebApplication.Application;
            }
        }

        public string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding).Replace("%2b", "+");
        }


        public void CompleteRequest()
        {
            _httpContext.ApplicationInstance.CompleteRequest();
        }

        public IDictionary Items
        {
            get
            {
                return _httpContext.Items;
            }
        }



        public ITARequest Request
        {
            get
            {
                return _request;
            }
        }

        public ITAResponse Response
        {
            get
            {
                return _response;
            }
        }


        public bool ExistCookie(string name)
        {
            try
            {
                HttpCookie cookie = _httpContext.Request.Cookies[name];
                if (cookie != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public ITACookie CreateCookie(string name)
        {
            return CreateCookie(name, null);
        }
        public ITACookie CreateCookie(string name, string encodekey)
        {
            if (ExistCookie(name))
            {
                return new MyCookie(_httpContext.Request.Cookies[name], encodekey);
            }
            else
            {
                return new MyCookie(new HttpCookie(name), encodekey);
            }
        }
        public void SaveCookie(ITACookie cookie)
        {
            MyCookie ck = cookie as MyCookie;
            _httpContext.Response.SetCookie(ck.Cookie);
        }
    }
}
