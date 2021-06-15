using MyAccess.Json;
using System;
using System.Collections.Generic;

namespace MyNet.SocketIO
{
    /// <summary>
    /// 执行ack回包
    /// </summary>
    public delegate void AckCallback(object data);
    /// <summary>
    /// Socket.IO事件参数
    /// </summary>
    public class IOEventArgs : EventArgs
    {
        private object _data;
        private AckCallback _callback;
        private bool _isask;
        public IOEventArgs(object data, AckCallback callback)
        {
            _data = data;
            _callback = callback;
            _isask = false;
        }
        public object Data
        {
            get { return _data; }
        }
        /// <summary>
        /// 调用回复
        /// </summary>
        /// <param name="data"></param>
        public void Ask(object data)
        {
            if (!_isask)
            {
                _isask = true;
                _callback?.Invoke(data);
            }
        }
 
        public T Cast<T>()
        {
            return Common.Converter.Cast<T>(_data);
        }
        public override string ToString()
        {
            return Json.Encode(_data);
        }
    }
}
