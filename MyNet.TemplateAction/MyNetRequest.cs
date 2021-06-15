using MyNet.Middleware.Http;
using System;
using System.IO;
using TemplateAction.Core;
using System.Collections.Specialized;

namespace MyNet.TemplateAction
{
    public class MyNetRequest : ITARequest
    {
        private ITAObjectCollection _form;
        private ITAObjectCollection _query;
        private HttpRequest _request;
        private IRequestFile[] _requestFiles;
        public MyNetRequest(HttpRequest request)
        {
            _request = request;
            _requestFiles = new MyNetWebRequestFile[_request.Files.Length];

            for (int i = 0; i < _request.Files.Length; i++)
            {
                _requestFiles[i] = new MyNetWebRequestFile(_request.Files[i]);
            }
            //初始化参数列表
            _form = new MyNetObjectCollection(request.Form);
            _query = new MyNetObjectCollection(request.Query);

        }
        public string ClientAgentIP
        {
            get
            {
                string result = _request.Headers.TryGet<string>("HTTP_X_FORWARDED_FOR");
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.Headers.TryGet<string>("REMOTE_ADDR");
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.Headers.TryGet<string>("WL-Proxy-Client-IP");
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.Headers.TryGet<string>("Proxy-Client-IP");
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.Headers.TryGet<string>("HTTP_CLIENT_IP");
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.Headers.TryGet<string>("X-Real-IP");
                }

                //主要看http代理服务器，要可以自己从请求头中找，当然匿名代理没有这些请求头
                if (string.IsNullOrEmpty(result))
                {
                    result = _request.RemoteEndPoint.Address.ToString();
                }
                return result;
            }
        }

        public string ClientIP
        {
            get
            {
                return _request.RemoteEndPoint.Address.ToString();
            }
        }


        public string ServerIP
        {
            get
            {
                return _request.LocalEndPoint.Address.ToString();
            }
        }

        public int ServerPort
        {
            get
            {
                return _request.LocalEndPoint.Port;
            }
        }

        public ITAObjectCollection Form
        {
            get
            {
                return _form;
            }
        }

        public string HttpMethod
        {
            get
            {
                return _request.HttpMethod.ToString();
            }
        }

        public Stream InputStream
        {
            get
            {
                return _request.InputStream;
            }
        }

        public string Path
        {
            get
            {
                return _request.Path;
            }
        }

        public ITAObjectCollection Query
        {
            get
            {
                return _query;
            }
        }

        public IRequestFile[] RequestFiles
        {
            get
            {
                return _requestFiles;
            }
        }
        public Uri Url
        {
            get
            {
                return _request.Url;
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                return _request.RefererUrl;
            }
        }

        public string UserAgent
        {
            get
            {
                return _request.UserAgent;
            }
        }

        public NameValueCollection Header
        {
            get
            {
                return _request.Headers;
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
                return new MyNetCookie(_request.Cookies[name], encodekey);
            }
            else
            {
                return new MyNetCookie(new HttpCookie(name), encodekey);
            }
        }

        public bool ExistCookie(string name)
        {
            return _request.Cookies.ExistCookie(name);
        }

    }
}
