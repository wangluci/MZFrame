using System;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class HttpSessionManager
    {
        private const int EndMinute = 20;
        private string DESPassword = Guid.NewGuid().ToString("N");
        /// <summary>  
        /// 保存在服务端的Session池,单例且唯一  
        /// </summary>  
        /// <remarks>  
        /// </sessionid></remarks>  
        private readonly ConcurrentDictionary<string, HttpSessionProxy> SessionPools = new ConcurrentDictionary<string, HttpSessionProxy>();

        /// <summary>  
        /// 自动销毁过期Session的线程  
        /// </summary>  
        private Thread mAutoDestroyTimeOutSessionThread;
        private object lockHelper = new object();
        private bool _loop = true;
        public HttpSessionManager(){

            //创建超时守护线程.  
            mAutoDestroyTimeOutSessionThread = new Thread(DestroyTimeOutSession);
            mAutoDestroyTimeOutSessionThread.IsBackground = true;
            mAutoDestroyTimeOutSessionThread.Start();
        }
        public void Close()
        {
            _loop = false;
        }
        /// <summary>  
        /// 销毁超时Session的方法  
        /// </summary>  
        private void DestroyTimeOutSession()
        {
            //1.查找过时的Seesion  
            while (_loop)            //无限循环  
            {
                Thread.Sleep(60000);//挂起1分钟后再执行   
                try
                {
                    //查找过时的Seesion   
                    DateTime now = DateTime.Now;

                    //x分钟的过期时间,所以客户端要每x分钟内发一次心跳包 
                    string[] oldSessions = SessionPools.Where(session => (now - session.Value.Session().LastConnTime).Minutes > EndMinute)
                                                  .Select(p => p.Key).ToArray();

                    //销毁  
                    foreach (string item in oldSessions)
                    {
                        RemoveSession(item);
                    }
                }
                catch { }

            }

        }
        public void RemoveSession(string id)
        {
            HttpSessionProxy session = null;
            if (SessionPools.TryRemove(id, out session))
            {
                TAEventDispatcher.Instance.Dispatch(new EndSessionEvent(session.Session()));
            }
        }
        public HttpSession GetSession(string id)
        {
            HttpSessionProxy proxy;
            if (SessionPools.TryGetValue(id, out proxy))
            {
                return proxy.Session();
            }
            return null;
        }
        /// <summary>  
        /// 创建HttpSession
        /// </summary>  
        public HttpSession CreateSession(HttpContext context)
        {
            HttpSession mySession = null;
            string SessionId = "";

            //如果用户还未得到SessionID,那么将会先还回SessionID,下次请求才是真正请求
            ITACookie mySessionCookie = context.CreateCookie("SessionId", DESPassword);
            bool isnewsession = false;
            if (mySessionCookie.IsEmpty())
            {
                isnewsession = true;
            }

            //设置SessionID值和重置Cookie过期时间
            if (!isnewsession)
            {
                try
                {
                    SessionId = mySessionCookie.GetValue();
                }
                catch
                {
                    isnewsession = true;
                }
            }

 
            if (isnewsession)
            {
                SessionId = Guid.NewGuid().ToString("N");
                mySessionCookie.SetExpires(EndMinute + 10);
                mySessionCookie.SetValue(SessionId);
                context.SaveCookie(mySessionCookie);
                //创建一个HttpSession  
                HttpSessionProxy tmpproxy = SessionPools.GetOrAdd(SessionId, (k) =>
                {
                    return new HttpSessionProxy(k);
                });
                return tmpproxy.Session();
            }
            if (mySessionCookie.Expires <= DateTime.Now.AddMinutes(EndMinute))
            {
                mySessionCookie.SetExpires(EndMinute + 10);
                context.SaveCookie(mySessionCookie);
            }
           
            //获取SessionID对应Session  
            //如果没有Session 就新建一个放到Session集合中  
            HttpSessionProxy proxy = SessionPools.GetOrAdd(SessionId, (k) =>
            {
                return new HttpSessionProxy(k);
            });
            mySession = proxy.Session();
            mySession.ResetConnTime();
            return mySession;
        }
    }
}
