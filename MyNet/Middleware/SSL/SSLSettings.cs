using System;
using System.Security.Authentication;
namespace MyNet.Middleware.SSL
{
    public abstract class SSLSettings
    {
        protected SSLSettings(SslProtocols enabledProtocols, bool checkCertificateRevocation)
        {
            this.EnabledProtocols = enabledProtocols;
            this.CheckCertificateRevocation = checkCertificateRevocation;
        }

        public SslProtocols EnabledProtocols { get; }

        public bool CheckCertificateRevocation { get; }
    }
}
