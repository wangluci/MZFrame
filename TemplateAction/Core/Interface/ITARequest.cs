using System;
using System.Collections.Specialized;
using System.IO;

namespace TemplateAction.Core
{
    public interface ITARequest
    {
        ITAObjectCollection Query { get;}
        ITAObjectCollection Form { get; }
        NameValueCollection Header { get; }
        string ServerIP { get; }
        int ServerPort { get; }
        string ClientIP { get; }

        string Path { get; }
        string HttpMethod { get; }
        string UserAgent { get; }
        /// <summary>
        /// 包含代理的Ip信息
        /// </summary>
        string ClientAgentIP { get; }
        IRequestFile[] RequestFiles { get; }
        Stream InputStream { get; }
        Uri Url { get; }
        Uri UrlReferrer { get; }

    }
}
