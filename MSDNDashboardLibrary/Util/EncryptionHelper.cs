using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;


namespace MSDNDashboard.Util
{
    public static class EncryptionHelper
    {
        public static ClientAssertionCertificate AssertionCert { get; set; }
        public static Dictionary<string, string> Configs { get; set; } 

        private static KeyVaultClient KVClient;

        public static KeyVaultClient GetKVClient()
        {
            return KVClient;
        }

        static EncryptionHelper()
        {
            Configs = new Dictionary<string, string>();
            Configs.Add("BlogsDbConnectionString", "not set");
            Configs.Add("BlogsRedisUri", "not set");
            Configs.Add("BlogsRedisAuthKey", "not set");
            Configs.Add("ProfileApiUri", "not set");
            Configs.Add("ProfileAppKey", "not set");
        }

        public static void InitilizeKV(string secretUri, string thumbprint, string clientId)
        {
            if (KVClient == null)
            {
                GetCert(thumbprint, clientId);
                KVClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

                var keys = new List<string>(Configs.Keys);

                foreach (var key in keys)
                {
                    Configs[key] = KVClient.GetSecretAsync(String.Format(secretUri,key)).Result.Value;
                }
            }
        }

        private static void GetCert(string thumbprint, string clientId)
        {
            var clientAssertionCertPfx = FindCertificateByThumbprint(thumbprint);
            AssertionCert = new ClientAssertionCertificate(clientId, clientAssertionCertPfx);
        }

        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority);

            var result = await context.AcquireTokenAsync(resource, AssertionCert);
            return result.AccessToken;
        }

        private static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint,
                    findValue, false); // Don't validate certs, since the test root isn't installed.
                if (col == null || col.Count == 0)
                    return null;
                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}