using MyNet.Handlers;
using MyNet.Middleware.Http;
using System;
using System.IO;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MyNetResponse : ITAResponse
    {
        private IContext _context;
        private HttpResponse _response;
        public MyNetResponse(IContext context, HttpResponse response)
        {
            _context = context;
            _response = response;
        }
        public int StatusCode
        {
            get
            {
                return _response.Status;
            }

            set
            {
                _response.Status = value;
            }
        }

        public string StatusDescription
        {
            get
            {
                return _response.StatusDescription;
            }

            set
            {
                _response.StatusDescription = value;
            }
        }
        public void SaveCookie(ITACookie cookie)
        {
            MyNetCookie ck = cookie as MyNetCookie;
            _response.Cookies.SetCookie(ck.GenerateHttpCookie());
        }
        public string ContentType
        {
            get
            {
                return _response.ContentType;
            }

            set
            {
                _response.ContentType = value;
            }
        }

        public Stream OutputStream
        {
            get
            {
                return _response.OutputStream;
            }
        }

        public void AppendHeader(string name, string value)
        {
            _response.Headers.Add(name, value);
        }

        public void BinaryWrite(byte[] buffer)
        {
            _response.Write(buffer);
        }

        public void Clear()
        {
            _response.Clear();
        }

        public void Write(string s)
        {
            _response.Write(s);
        }

        public void Redirect(string url)
        {
            _response.Redirect(_context, url);
        }
        public void End()
        {
            _response.End(_context);
        }
        public void End503()
        {
            _response.End503(_context);
        }
    }
}
