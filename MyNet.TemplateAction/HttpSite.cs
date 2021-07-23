using MyNet.Common;
using MyNet.Handlers;
using MyNet.Loop.Scheduler;
using MyNet.Middleware.Http;
using MyNet.Middleware.SSL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Core.Dispatcher;
using TemplateAction.Label;

namespace MyNet.TemplateAction
{
    public class HttpSite : BaseDisposable
    {
        protected HttpConfig _httpConfig = new HttpConfig();
        /// <summary>
        /// 过滤的扩展名
        /// </summary>
        protected static readonly string[] FILTER_EXT =
        {
            TAUtility.FILE_EXT,".config",".csproj",".cs",".dll",".exe",".pdb"
        };
        /// <summary>
        /// Web目录
        /// </summary>
        protected string _wwwroot;
        protected string _rootpath;
        public string RootPath
        {
            get { return _rootpath; }
        }
        protected string _ip;
        protected int _port;

        protected HttpSessionManager _sessionManager;
        public HttpSessionManager GetSessionManager()
        {
            return _sessionManager;
        }

        protected TASiteApplication _app;
        public TASiteApplication Application
        {
            get { return _app; }
        }
        protected ServerBootstrap _boot;
        /// <summary>
        /// 大于2M的静态文件不缓存
        /// </summary>
        protected int _maxcachestaticfile = 2097152;
        protected string _404Url = string.Empty;
        protected string _defaultDoucument = string.Empty;
        /// <summary>
        /// '/'默认指向文件
        /// </summary>
        public string DefaultDoucument
        {
            get { return _defaultDoucument; }
            set { _defaultDoucument = value; }
        }
        protected bool _gzip;
        /// <summary>
        /// 是否开户动态页面的gzip
        /// </summary>
        public bool GZIP
        {
            get { return _gzip; }
            set { _gzip = value; }
        }
        protected int _workThread;
        /// <summary>
        /// 工作线程数
        /// </summary>
        public int WorkThread
        {
            get { return _workThread; }
            set { _workThread = value; }
        }
        protected SSLServerSettings _sslsettings;
        public SSLServerSettings SSLSettings
        {
            get { return _sslsettings; }
            set { _sslsettings = value; }
        }
        public const int WORKER_STATE_SHUTDOWN = 0;
        public const int WORKER_STATE_STARTED = 1;
        /// <summary>
        /// 0为关闭中，1为运行中
        /// </summary>
        private volatile int _workerState;
        private HashSet<string> _filterext;

        public HttpSite(string ip, int port)
        {
            _filterext = new HashSet<string>();
            foreach (string s in FILTER_EXT)
            {
                _filterext.Add(s.ToLower());
            }
            _workThread = 10;
            _workerState = WORKER_STATE_SHUTDOWN;
            _gzip = false;
            _ip = ip;
            _port = port;
            _rootpath = AppDomain.CurrentDomain.BaseDirectory;
            _wwwroot = Path.Combine(_rootpath, "Web");
            TemplateApp.Instance.Init(_wwwroot);
            //注册action监听
            TAEventDispatcher.Instance.Register(new DefaultHandler<ActionEvent>(evt =>
            {
                OnAction(evt.Context, evt.Request);
            }));
            //注册Session开始
            TAEventDispatcher.Instance.Register(new DefaultHandler<StartSessionEvent>(evt =>
            {
                this.OnSessionStart(evt.Session);
            }));
            //注册Session结束
            TAEventDispatcher.Instance.Register(new DefaultHandler<StartSessionEvent>(evt =>
            {
                this.OnSessionEnd(evt.Session);
            }));


            //程序退出监听
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                OnUnManDisposed();
            };
            Console.CancelKeyPress += (s, e) =>
            {
                OnUnManDisposed();
            };
        }
        /// <summary>
        /// 运行站点
        /// </summary>
        public void Run()
        {
            int originalWorkerState = Interlocked.CompareExchange(ref _workerState, WORKER_STATE_STARTED, WORKER_STATE_SHUTDOWN);
            if (originalWorkerState != WORKER_STATE_SHUTDOWN)
            {
                return;
            }

            TAEventDispatcher.Instance.RegisterLoadBefore<TASiteApplication>(app =>
            {
                OnSiteConfig(app);
            });
            _app = new TASiteApplication().Init(_rootpath);
            //站点开始
            OnSiteStart();
            _sessionManager = new HttpSessionManager();
            _boot = new ServerBootstrap();
            //初始化中间件
            _boot.Handler(new InitializerHandler(c =>
            {
                ConfigurePipelineBefore(c.Pipeline);
                if (_sslsettings != null)
                {
                    c.Pipeline.AddLast(new SSLHandler(_sslsettings));
                }
                c.Pipeline.AddLast(new HttpServerHandler(_httpConfig));
                ConfigurePipeline(c.Pipeline);
                c.Pipeline.AddLast(new MvcHandler());
                ConfigurePipelineAfter(c.Pipeline);
            }));
            EventLoopGroup acceptGroup = new EventLoopGroup();
            EventLoopGroup workGroup = new EventLoopGroup(_workThread);
            _boot.Group(acceptGroup, workGroup);
            _boot.LaunchChannel(_ip, _port).Wait();
        }
        /// <summary>
        /// 停止站点
        /// </summary>
        public void Stop()
        {
            OnUnManDisposed();
        }
        /// <summary>
        /// Http请求处理
        /// </summary>
        private void OnAction(IContext context, HttpRequest request)
        {
            bool rootdef = false;
            if (!string.IsNullOrEmpty(_defaultDoucument) && request.Path.Equals("/"))
            {
                rootdef = true;
            }

            //判断是否为静态文件
            if (TAUtility.IsStaticFile(request.Path) || rootdef)
            {

                //处理静态文件
                string path = this.MapPath(request.Path.ToLower());
                if (rootdef)
                {
                    path += "\\" + _defaultDoucument;
                }
                string ext = Path.GetExtension(path);
                //过滤掉重要的程序文件
                if (_filterext.Contains(ext.ToLower()))
                {
                    new HttpResponse().End404(context);
                    return;
                }

                HttpResponse response = new HttpResponse();
                response.ContentType = FileContentType.GetMimeType(ext);
                //支持断点下载续传
                if (request.Headers.ContainsKey("Range"))
                {
                    response.Headers.Add("Accept-Ranges", "bytes");
                    string reqrange = request.Headers["Range"];
                    RangeDown(context, request, response, path, reqrange);
                    return;
                }

                byte[] data = StaticFileCache.Instance.GetGZipStaticFile(path, _maxcachestaticfile);
                response.Gzip = false;
                response.GzipHeader = true;
                if (data == null)
                {
                    new HttpResponse().End404(context);
                }
                else
                {
                    response.Write(data);
                    response.End(context);
                }
            }
            else
            {
                this.ExcuteAction(context, request);
            }
        }
        private void RangeDown(IContext context, HttpRequest request, HttpResponse response, string path, string range)
        {
            if (range.StartsWith("bytes="))
            {
                FileStream fs = new FileStream(path, FileMode.Open);//初始化文件流
                try
                {
                    string r = range.Substring(6);
                    int spi = r.IndexOf("-");
                    if (spi < 0)
                    {
                        throw new Exception("range数据异常");
                    }
                    string ss = r.Substring(0, spi);
                    string es = r.Substring(spi + 1);
                    if (string.IsNullOrEmpty(ss))
                    {
                        int ei = Converter.Cast<int>(es);
                        int si = (int)(fs.Length - ei);
                        if (si < 0)
                        {
                            throw new Exception("si数据异常");
                        }
                        byte[] filedata = new byte[ei];//初始化字节数组
                        fs.Seek(-ei, SeekOrigin.End);
                        fs.Read(filedata, 0, ei);//读取流中数据到字节数组中
                        response.Headers.Add("bytes", si + "-" + ei + "/" + fs.Length);
                        response.Write(filedata);
                    }
                    else if (string.IsNullOrEmpty(es))
                    {
                        int si = Converter.Cast<int>(ss);
                        int ei = (int)fs.Length - 1;
                        int tlen = (int)(fs.Length - si);
                        byte[] filedata = new byte[tlen];
                        fs.Seek(si, SeekOrigin.Begin);
                        fs.Read(filedata, 0, tlen);
                        response.Headers.Add("bytes", si + "-" + ei + "/" + fs.Length);
                        response.Write(filedata);
                    }
                    else
                    {
                        int si = Converter.Cast<int>(ss);
                        int ei = Converter.Cast<int>(es);
                        int tlen = ei - si + 1;
                        byte[] filedata = new byte[tlen];
                        fs.Seek(si, SeekOrigin.Begin);
                        fs.Read(filedata, 0, tlen);
                        response.Headers.Add("bytes", si + "-" + ei + "/" + fs.Length);
                        response.Write(filedata);
                    }
                    response.End206(context);
                }
                catch (Exception)
                {
                    response.End416(context);
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                response.End416(context);
            }
        }
        /// <summary>
        /// 执行站点行为
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        private void ExcuteAction(IContext context, HttpRequest request)
        {
            HttpResponse response = new HttpResponse();
            if (_gzip)
            {
                response.Gzip = true;
                response.GzipHeader = true;
            }

            HttpContext httpcontext = new HttpContext(context, request, response, this);
            TAEventDispatcher.Instance.Dispatch(new ContextCreatedEvent(httpcontext));
            TARequestHandleBuilder builder = _app.Route(httpcontext);
            if (builder == null)
            {
                response.End404(context);
                return;
            }
            if (builder.Async != null)
            {
                MyNetAsyncResult asynrs = new MyNetAsyncResult(builder);
                if (builder.Async.AsyncTimeout > 0)
                {
                    //设定超时
                    ITriggerRunnable timeoutrun = context.Loop.Schedule(asw =>
                     {
                         asw.Timeout();
                     }, asynrs, builder.Async.AsyncTimeout * 1000);
                    asynrs.SetRunnable(timeoutrun);
                }
                //开始执行异步结果
                builder.StartAsync(asynrs);
                return;
            }
            else
            {
                builder.BuildAndExcute().Output();
            }
            response.End(context);
        }

        /// <summary>
        /// 可重写此方法，重定向静态文件,例:重定向上传的图片文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string MapPath(string path)
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

            return Path.Combine(_wwwroot, path);
        }


        protected virtual void OnSessionStart(ITASession session) { }
        protected virtual void OnSessionEnd(ITASession session) { }
        protected virtual void OnSiteConfig(TAAbstractApplication app) { }
        protected virtual void OnSiteStart() { }
        protected virtual void OnSiteStop() { }
        protected virtual void ConfigurePipelineBefore(IChannelPipeline pipeline) { }
        protected virtual void ConfigurePipeline(IChannelPipeline pipeline) { }
        protected virtual void ConfigurePipelineAfter(IChannelPipeline pipeline) { }
        protected override void OnUnManDisposed()
        {
            int originalWorkerState = Interlocked.CompareExchange(ref _workerState, WORKER_STATE_SHUTDOWN, WORKER_STATE_STARTED);
            if (originalWorkerState != WORKER_STATE_STARTED)
            {
                return;
            }
            //执行站点终止任务
            this.OnSiteStop();
            _boot.Dispose();
            _sessionManager.Close();
            _boot = null;
            _sessionManager = null;
        }
    }
}
