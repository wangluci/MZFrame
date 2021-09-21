using MyAccess.DB.Attr;
using System;

namespace AuthService
{
    public class SysLog
    {
        [ID(true)]
        public long SysLogID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public long UserId { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 日志类型0为成功，1为失败
        /// </summary>
        public byte LogType { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public string Info { get; set; }
    }
}
