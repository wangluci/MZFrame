using System;

namespace MyNet.SocketIO
{
    /// <summary>
    /// socket.io握手回复包
    /// </summary>
    public class AuthData
    {
        public string sid { get; set; }
        public string[] upgrades { get; set; }
        public int pingInterval { get; set; }
        public int pingTimeout { get; set; }
    }
}
