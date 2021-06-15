using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
namespace MyNet.Middleware.SSL
{
    public class SSLServerSettings: SSLSettings
    {
        public SSLServerSettings(X509Certificate certificate)
            : this(certificate, false)
        {
        }

        public SSLServerSettings(X509Certificate certificate, bool negotiateClientCertificate)
            : this(certificate, negotiateClientCertificate, false)
        {
        }

        public SSLServerSettings(X509Certificate certificate, bool negotiateClientCertificate, bool checkCertificateRevocation)
            : this(certificate, negotiateClientCertificate, checkCertificateRevocation, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12)
        {
        }

        public SSLServerSettings(X509Certificate certificate, bool negotiateClientCertificate, bool checkCertificateRevocation, SslProtocols enabledProtocols)
            : base(enabledProtocols, checkCertificateRevocation)
        {
            this.Certificate = certificate;
            this.NegotiateClientCertificate = negotiateClientCertificate;
        }

        public X509Certificate Certificate { get; }

        public bool NegotiateClientCertificate { get; }
    }
}
