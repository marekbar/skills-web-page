using System;
using System.Security.Cryptography.X509Certificates;

namespace SkillsAndTasksDesktopClient
{
    /// <summary>
    /// Handles client's side of certificate
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Instance of Certificate
        /// </summary>
        private static Certificate instance = null;

        /// <summary>
        /// Certificate filename
        /// </summary>
        private String filename = "Client.pfx";

        /// <summary>
        /// Certificate password
        /// </summary>
        private String password = "qqlka123";

        /// <summary>
        /// Certificate readable name to see in user personal cert store
        /// </summary>
        private String name = "Umiejętności i zadania - Marek Bar 33808";

        /// <summary>
        /// Certificate fingerprint
        /// </summary>
        public static String FingerPrint = "f4 30 7a 98 b9 cd 40 9f 9c 18 ea 1f 56 65 c4 a2 10 43 84 21";

        /// <summary>
        /// Gets Certificate instance
        /// </summary>
        public static Certificate Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Certificate();
                }
                return instance;
            }
        }

        /// <summary>
        /// hidden constructor
        /// </summary>
        private Certificate()
        {

        }

        /// <summary>
        /// Determinates if require certificate is installed or not
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                try
                {
                    //certificate key store- personal of current user
                    var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                    //opens read only just to check its presence
                    store.Open(OpenFlags.ReadOnly);

                    ///find certificate by its fingerprint
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, FingerPrint, true);

                    //if any match then found
                    if (certs.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Install certificate if not present in current user personal certificate store
        /// </summary>
        /// <returns>bool</returns>
        public bool InstallWhenNeeded()
        {
            try
            {
                if (!IsInstalled)
                {
                    var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);

                    var certificate = new X509Certificate2(filename, password,
                        X509KeyStorageFlags.PersistKeySet //Klucza skojarzonego z pliku PFX jest zachowywane podczas importowania certyfikatu.
                        | X509KeyStorageFlags.Exportable
                        );
                    certificate.FriendlyName = name;
                    store.Add(certificate);


                    store.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets X509 certificate from current user personnal keystore
        /// </summary>
        /// <param name="IsFindByMail">true - find by certificate thumb print, false - find by issuer mail address</param>
        /// <returns>X509Certificate2</returns>
        public X509Certificate2 GetCertificate(bool IsFindByMail = false)
        {
            X509Certificate2 cert;

            try
            {
                //this is where client has its cert
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                //open it
                store.Open(OpenFlags.ReadOnly);

                //find by
                if (IsFindByMail)
                {
                    //get it from keystore by mail
                    cert = store.Certificates.Find(X509FindType.FindBySubjectName, "marekbar1985@gmail.com", true)[0];
                }
                else
                {
                    //get it from keystore by thumb print
                    cert = store.Certificates.Find(X509FindType.FindByThumbprint, FingerPrint, true)[0];
                }
                //close key store - obligatory task
                store.Close();

                //gimmie that thing
                return cert;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
