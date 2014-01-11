using SkillsAndTasksDesktopClient;
using SkillsAndTasksWebPageWcfConsumer.Service;
/*
 * Skills and tasks project
 * Klient WinForms z komunikacją przez HTTPS i autoryzacją certyfikatem
 * Author: Marek Bar 33808
 * Wyższa Szkoła Inforatyki i Zarządzania w Rzeszowie
 * marekbar1985@gmail.com
 */
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace WCF
{
    /// <summary>
    /// Service factory
    /// </summary>
    public static class Service
    {
        /// <summary>
        /// Create service based on configuration   in app.config when true
        /// otherwise use pure programatically created web service client
        /// </summary>
        private static bool FROM_APP_CONFIG = false;

        /// <summary>
        /// WCF web service address
        /// </summary>
        private static String SERVICE_ADDRESS = "https://127.0.0.1/MBService.svc";

        /// <summary>
        /// Creates new WCF service client
        /// </summary>
        /// <returns>MBService.MBServiceClient</returns>
        public static MBServiceClient Create()
        {
            SkillsAndTasksWebPageWcfConsumer.Service.MBServiceClient client = null;
            try
            {
                if (FROM_APP_CONFIG)
                {
                    client = new SkillsAndTasksWebPageWcfConsumer.Service.MBServiceClient();
                }
                else
                {
                    //client = Service.GetClientWithSSL();
                    client = Service.GetClientWithCertificate();
                }
            }
            catch(Exception ex)
            {
                client = null;
            }
            return client;
        }

        /// <summary>
        /// Get client with ssl transport and certificate authentication
        /// </summary>
        /// <remarks>
        /// the line below is copied by copy-paste and won't work
        //‎/ "f4 30 7a 98 b9 cd 40 9f 9c 18 ea 1f 56 65 c4 a2 10 43 84 21"
        /// repairing after copy-paste that way won't help
        /// "‎f4307a98b9cd409f9c18ea1f5665c4a210438421"
        /// need manually rewrite
        /// </remarks>
        /// <see cref="Should read this: http://msdn.microsoft.com/pl-pl/library/ms576355%28v=vs.110%29.aspx"/>
        /// <returns>MBService.MBServiceClient</returns>
        private static MBServiceClient GetClientWithCertificate()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(checkCertificate);
            var binding = Service.getBinding(HttpClientCredentialType.Certificate);

            var client = new MBServiceClient(binding, new EndpointAddress(getAddress()));

            client.ClientCredentials.ClientCertificate.Certificate = Certificate.Instance.GetCertificate();

            return client;
        }

        /// <summary>
        /// Check client certificate
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="certificate">X509Certificate</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="sslPolicyErrors">SslPolicyErrors</param>
        /// <see cref="http://msdn.microsoft.com/pl-pl/library/system.net.security.remotecertificatevalidationcallback%28v=vs.110%29.aspx"/>
        /// <returns>bool</returns>
        private static bool checkCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }


            //forget about communication with unauthenticated servers
            return false;
        }

        /// <summary>
        /// Gets binding for SSL transport
        /// </summary>
        /// <param name="clientCredential">HttpClientCredentialType - choose None or Certificate</param>
        /// <returns>WSHttpBinding</returns>
        private static WSHttpBinding getBinding(HttpClientCredentialType clientCredential)
        {
            var binding = new WSHttpBinding();
            binding.Name = "WSHttpBinding_IMBService";

            //SSL
            binding.Security.Mode = SecurityMode.Transport;

            binding.Security.Transport.ClientCredentialType = clientCredential;
            binding.Security.Transport.Realm = "";
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

            //timeouts
            binding.CloseTimeout = TimeSpan.FromMinutes(5);
            binding.OpenTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            binding.SendTimeout = TimeSpan.FromMinutes(5);

            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.UseDefaultWebProxy = true;

            //quotas
            binding.ReaderQuotas.MaxDepth = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

            //and finally get binding
            return binding;
        }

        /// <summary>
        /// Gets WCF service client for SSL transport security
        /// </summary>
        /// <returns>MBService.MBServiceClient</returns>
        private static MBServiceClient GetClientWithSSL()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ignoreCertificate);
            var binding = Service.getBinding(HttpClientCredentialType.None);
            return new MBServiceClient(binding, new EndpointAddress(getAddress()));
        }
        /// <summary>
        /// Fake certificate check
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="certificate">X509Certificate</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="sslPolicyErrors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private static bool ignoreCertificate(object sender, 
            System.Security.Cryptography.X509Certificates.X509Certificate certificate, 
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static String getAddress()
        {
            return SERVICE_ADDRESS;
        }
    }
}