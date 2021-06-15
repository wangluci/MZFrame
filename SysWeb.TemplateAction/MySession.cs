using System;
using TemplateAction.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SysWeb.TemplateAction
{
    public class MySession : ITASession
    {
        private DateTime mLastConnTime;
        private string mSessionID;
        public string SessionId { get { return mSessionID; } }
        private ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();
        private object _lock = new object();
        public MySession()
        {
            mLastConnTime = DateTime.Now;
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

        public IEnumerable<string> Keys
        {
            get
            {
                return _dict.Keys;
            }
        }

        internal void ResetConnTime()
        {
            mLastConnTime = DateTime.Now;
        }

        public bool Remove(string name)
        {
            object t;
            return _dict.TryRemove(name, out t);
        }

        public void SetObject(string name, object value)
        {
            _dict[name] = value;
        }

        public void SetInt32(string name, int value)
        {
            _dict[name] = value;
        }

        public void SetString(string name, string value)
        {
            _dict[name] = value;
        }

        public int? GetInt32(string name)
        {
            object targetObj;
            if(_dict.TryGetValue(name,out targetObj))
            {
                if (targetObj != null)
                {
                    if(targetObj is int)
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
            if (_dict.TryGetValue(name, out targetObj))
            {
                if (targetObj != null)
                {
                    if(targetObj is string)
                    {
                        return (string)targetObj;
                    }
                    return targetObj.ToString();
                }
            }
            return null;
        }

        public T GetObject<T>(string name) where T : class
        {
            object targetObj;
            if (_dict.TryGetValue(name, out targetObj))
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
