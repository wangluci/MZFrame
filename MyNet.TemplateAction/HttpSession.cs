using System;
using TemplateAction.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyNet.TemplateAction
{
    public class HttpSession : ITASession
    {
        private DateTime mLastConnTime;
        private string mSessionID;
        private ConcurrentDictionary<string, object> _dic = new ConcurrentDictionary<string, object>();
        public HttpSession()
        {
            mLastConnTime = DateTime.Now;
        }


        public bool Remove(string name)
        {
            object t;
            return _dic.TryRemove(name, out t);
        }

 
        /// <summary>  
        /// 会话ID  
        /// </summary>  
        public string SessionID
        {
            get { return mSessionID; }
            internal set { mSessionID = value; }
        }


        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        public DateTime LastConnTime
        {
            get { return mLastConnTime; }
        }

        public string SessionId
        {
            get
            {
                return mSessionID;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _dic.Keys;
            }
        }

        internal void ResetConnTime()
        {
            mLastConnTime = DateTime.Now;
        }
        public void SetObject(string name, object value)
        {
            _dic[name] = value;
        }

        public void SetInt32(string name, int value)
        {
            _dic[name] = value;
        }

        public void SetString(string name, string value)
        {
            _dic[name] = value;
        }

        public int? GetInt32(string name)
        {
            object targetObj;
            if (_dic.TryGetValue(name, out targetObj))
            {
                if (targetObj != null)
                {
                    if (targetObj is int)
                    {
                        return (int)targetObj;
                    }
                    int rt;
                    if (int.TryParse(targetObj.ToString(), out rt))
                    {
                        return rt;
                    }
                }

            }
            return null;
        }

        public string GetString(string name)
        {
            object targetObj;
            if (_dic.TryGetValue(name, out targetObj))
            {
                return Convert.ToString(targetObj);
            }
            else
            {
                return null;
            }
        }

        public T GetObject<T>(string name) where T : class
        {
            object targetObj;
            if (_dic.TryGetValue(name, out targetObj))
            {
                return targetObj as T;
            }
            else
            {
                return null;
            }
        }
    }
}
