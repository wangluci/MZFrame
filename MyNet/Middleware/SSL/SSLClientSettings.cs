using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MyNet.Middleware.SSL
{
    public class SSLClientSettings : SSLSettings
    {
        public SSLClientSettings(string targetHost)
            : this(targetHost, new List<X509Certificate>())
        {
        }

        public SSLClientSettings(string targetHost, List<X509Certificate> certificates)
            : this(false, certificates, targetHost)
        {
        }

        public SSLClientSettings(bool checkCertificateRevocation, List<X509Certificate> certificates, string targetHost)
            : this(SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, checkCertificateRevocation, certificates, targetHost)
        {
        }

        public SSLClientSettings(SslProtocols enabledProtocols, bool checkCertificateRevocation, List<X509Certificate> certificates, string targetHost)
            :base(enabledProtocols, checkCertificateRevocation)
        {
            this.X509CertificateCollection = new X509CertificateCollection(certificates.ToArray());
            this.TargetHost = targetHost;
            this.Certificates = certificates.AsReadOnly();
        }

        internal X509CertificateCollection X509CertificateCollection { get; set; }

        public IReadOnlyCollection<X509Certificate> Certificates { get; }

        public string TargetHost { get; }
    }
}
