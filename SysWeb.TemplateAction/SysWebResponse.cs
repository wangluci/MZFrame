
using System;
using System.IO;
using System.Web;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class SysWebResponse : ITAResponse
    {
        private HttpResponse _response;
        public SysWebResponse(HttpResponse response)
        {
            _response = response;
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

        public int StatusCode
        {
            get
            {
                return _response.StatusCode;
            }

            set
            {
                _response.StatusCode = value;
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

        public void AppendHeader(string name, string value)
        {
            _response.AppendHeader(name, value);
        }

        public void BinaryWrite(byte[] buffer)
        {
            _response.BinaryWrite(buffer);
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
            _response.Redirect(url, false);
        }
    }
}
