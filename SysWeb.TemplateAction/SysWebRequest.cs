
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using TemplateAction.Common;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class SysWebRequest : ITARequest
    {
        private HttpRequest _req;
        private ITAObjectCollection _form;
        private ITAObjectCollection _query;
        private IRequestFile[] _requestFiles;
        public SysWebRequest(HttpRequest request)
        {
            _req = request;
            //初始化参数列表
            _form = new SysWebObjectCollection(_req.Form);
            _query = new SysWebObjectCollection(_req.QueryString);
         
            HttpFileCollection files = _req.Files;
            _requestFiles = new IRequestFile[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                _requestFiles[i] = new SysWebRequestFile(files[i]);
            }
        }
        public string ClientAgentIP
        {
            get
            {
                string result = _req.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (null == result || result == String.Empty)
                {
                    result = _req.ServerVariables["REMOTE_ADDR"];
                }
                if (null == result || result == String.Empty)
                {
                    result = _req.UserHostAddress;
                }
                return result;
            }
        }

        public string ServerIP
        {
            get
            {
                return _req.ServerVariables["Local_Addr"];
            }
        }

        public int ServerPort
        {
            get
            {
                return TAConverter.Cast<int>(_req.ServerVariables["SERVER_PORT"], 0);
            }
        }
        public string ClientIP
        {
            get
            {
                return _req.UserHostAddress;
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
                return _req.HttpMethod;
            }
        }

        public Stream InputStream
        {
            get
            {
                return _req.InputStream;
            }
        }

        public string Path
        {
            get
            {
                return _req.Path;
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
                return _req.Url;
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                return _req.UrlReferrer;
            }
        }


        public string UserAgent
        {
            get
            {
                return _req.UserAgent;
            }
        }

        public NameValueCollection Header
        {
            get
            {
                return _req.Headers;
            }
        }

    }
}
