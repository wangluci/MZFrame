using System;
using System.Text;

namespace MyNet.MQ
{
    public class IdGenerater
    {
        private long _port;
        private string _ip;
        private long sequence = 0L;

        private static long twepoch = 1288834974657L;
        /// <summary>
        /// 序列号位数
        /// </summary>
        private static long sequenceBits = 12L;
        private long sequenceMask = -1L ^ (-1L << (int)sequenceBits);
        private long lastTimestamp = -1L;
        private static object syncRoot = new object();

        public IdGenerater(int port, string ip)
        {
            _port = port;
            _ip = ip;
        }

        public string nextId()
        {
            lock (syncRoot)
            {
                long timestamp = timeGen();

                if (timestamp < lastTimestamp)
                {
                    throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for %d milliseconds", lastTimestamp - timestamp));
                }

                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0L;
                }

                lastTimestamp = timestamp;
                StringBuilder sb = new StringBuilder();
                sb.Append(timestamp - twepoch);
                sb.Append(_ip);
                sb.Append(_port);
                sb.Append(sequence);
                return sb.ToString();
            }
        }

        protected long tilNextMillis(long lastTimestamp)
        {
            long timestamp = timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = timeGen();
            }
            return timestamp;
        }

        protected long timeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
