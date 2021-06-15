using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpRequest : ITARequest
    {
        private HttpRequest _request;
        private TANetCoreHttpQueryCollection _query;
        private TANetCoreHttpFormCollection _form;
        private TANetCoreHttpHeader _header;
        private IRequestFile[] _requestFiles;
        private Uri _url;
        private Uri _urlReferrer;
        private string _serverip;
        private string _clientip;
        public TANetCoreHttpRequest(HttpContext context)
        {
            _request = context.Request;
            _header = new TANetCoreHttpHeader(_request.Headers);
            _query = new TANetCoreHttpQueryCollection(_request.Query);

            if (_request.HasFormContentType)
            {
                IFormCollection fc = _request.ReadFormAsync().GetAwaiter().GetResult();
                _form = new TANetCoreHttpFormCollection(fc);
                int filecount = _request.Form.Files.Count;
                _requestFiles = new TANetCoreHttpFile[filecount];
                for (int i = 0; i < filecount; i++)
                {
                    _requestFiles[i] = new TANetCoreHttpFile(_request.Form.Files[i]);
                }
            }
        }
        public ITAObjectCollection Query
        {
            get { return _query; }
        }

        public ITAObjectCollection Form
        {
            get { return _form; }
        }

        public NameValueCollection Header
        {
            get { return _header; }
        }

        public string ServerIP
        {
            get
            {
                if (_serverip == null)
                {
                    _serverip = _request.HttpContext.Connection.LocalIpAddress.ToString();
                }
                return _serverip;
            }

        }

        public int ServerPort
        {
            get { return _request.HttpContext.Connection.LocalPort; }
        }

        public string ClientIP
        {
            get
            {
                if (_clientip == null)
                {
                    _clientip = _request.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                return _clientip;
            }
        }
        public Uri Url
        {
            get
            {
                if (_url == null)
                {
                    _url = new Uri(string.Format("{0}://{1}{2}{3}", _request.Scheme, _request.Host, _request.Path, _request.QueryString));
                }
                return _url;
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                if (_urlReferrer == null)
                {
                    _urlReferrer = new Uri(_request.Headers["Referer"].ToString());
                }
                return _urlReferrer;
            }
        }

        public string Path
        {
            get { return _request.Path; }
        }

        public string HttpMethod
        {
            get { return _request.Method; }
        }

        public string UserAgent
        {
            get { return _request.Headers["User-Agent"].ToString(); }
        }

        public string ClientAgentIP
        {
            get { return ClientIP; }
        }

        public IRequestFile[] RequestFiles
        {
            get { return _requestFiles; }
        }

        public Stream InputStream
        {
            get { return _request.Body; }
        }

    }
}
