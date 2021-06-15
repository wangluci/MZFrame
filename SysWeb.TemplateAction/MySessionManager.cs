using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    /// <summary>
    /// 自定义的Session
    /// </summary>
    public class MySessionManager
    {
        private const int EndMinute = 20;
        private string DESPassword = Guid.NewGuid().ToString("N");
        /// <summary>  
        /// 保存在服务端的Session池,单例且唯一  
        /// </summary>  
        /// <remarks>  
        /// Dictionary<sessionid mysession="">   
        /// </sessionid></remarks>  
        private static readonly ConcurrentDictionary<string, MySessionProxy> SessionPools = new ConcurrentDictionary<string, MySessionProxy>();
        private readonly static MySessionManager mSingleton = new MySessionManager();
        public static MySessionManager Instance()
        {
            return mSingleton;
        }
        static MySessionManager() { }
        private MySessionManager()
        {
            //创建超时守护线程.  
            mAutoDestroyTimeOutSessionThread = new Thread(DestroyTimeOutSession);
            mAutoDestroyTimeOutSessionThread.IsBackground = true;
            mAutoDestroyTimeOutSessionThread.Start();
        }
        /// <summary>  
        /// 自动销毁过期Session的线程  
        /// </summary>  
        private Thread mAutoDestroyTimeOutSessionThread;



        /// <summary>  
        /// 销毁超时Session的方法  
        /// </summary>  
        private void DestroyTimeOutSession()
        {
            //1.查找过时的Seesion  
            while (true)            //无限循环  
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
            MySessionProxy session = null;
            if (SessionPools.TryRemove(id, out session))
            {
                TAEventDispatcher.Instance.Dispatch(new EndSessionEvent(session.Session()));
            }
        }
        public MySession GetSession(string id)
        {
            MySessionProxy proxy;
            if (SessionPools.TryGetValue(id, out proxy))
            {
                return proxy.Session();
            }
            return null;
        }
        /// <summary>  
        /// 创建MySession
        /// </summary>  
        public MySession CreateSession(ITAContext context)
        {
            MySession mySession = null;
            string SessionId = null;

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
                //10分钟同免保存
                mySessionCookie.SetExpires(EndMinute + 10);
                mySessionCookie.SetValue(SessionId);
                context.SaveCookie(mySessionCookie);
                MySessionProxy tmpproxy = SessionPools.GetOrAdd(SessionId, (k) =>
                {
                    return new MySessionProxy(k);
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
            MySessionProxy proxy = SessionPools.GetOrAdd(SessionId, (k) =>
            {
                return new MySessionProxy(k);
            });
            mySession = proxy.Session();
            mySession.ResetConnTime();
            return mySession;
        }


    }
}
