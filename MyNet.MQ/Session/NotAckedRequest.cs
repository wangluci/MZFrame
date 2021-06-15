using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNet.MQ.Session
{
    public class NotAckedRequest : IComparable
    {
        private ushort _ackid;
        public ushort AckId { get { return _ackid; } }
        private long _overtime;
        public long OverTime { get { return _overtime; } }
        public NotAckedRequest(ushort ackid,long overtime)
        {
            _ackid = ackid;
            _overtime = overtime;
        }
        public int CompareTo(object obj)
        {
            NotAckedRequest cmp = obj as NotAckedRequest;
            if (cmp == null)
            {
                return -1;
            }
            return this._overtime.CompareTo(cmp._overtime);
        }
    }
}
