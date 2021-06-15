using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
namespace MyNet.Common
{
    public class PropertyCollection
    {
        private ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();
        public bool TryAdd(string key, object value)
        {
            return _dict.TryAdd(key, value);
        }
        public bool TryRemove(string key,out object value)
        {
            return _dict.TryRemove(key, out value);
        }
        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T TryGet<T>(string key)
        {
            return this.TryGet<T>(key, default(T));
        }

        /// <summary>
        /// 获取指定键的值
        /// 失败则返回defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T TryGet<T>(string key, T defaultValue)
        {
            object value;
            if (_dict.TryGetValue(key, out value))
            {
                return Converter.Cast<T>(value);
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
