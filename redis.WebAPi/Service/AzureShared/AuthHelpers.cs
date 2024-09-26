using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;

namespace redis.WebAPi.Service.AzureShared
{
    public class AuthHelpers
    {
        public static async Task<X509Certificate2> GetCertificateFromKeyVaultAsync(string certificateName,
            Uri clientCertKeyVaultUri,
            string tenantId)
        {
            RedisFunctionalTestKeyVaultObjectsRetriever keyVaultObjectsRetriever = new RedisFunctionalTestKeyVaultObjectsRetriever(tenantId, clientCertKeyVaultUri);
            TokenCredential credential = new DefaultAzureCredential();
            return await keyVaultObjectsRetriever.GetCertificateWithCredentialAsync(certificateName, credential);

        }

        public static X509Certificate2 GetX509Certificate2GenevaMetrics(Uri keyVaultUri)
        {
            try
            {
                SecretClient secretClient = new(vaultUri: keyVaultUri, credential: new DefaultAzureCredential());
                KeyVaultSecret secret = secretClient.GetSecret("BenchmarkToolGeneva");
                return new X509Certificate2(Convert.FromBase64String(secret.Value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetAccessTokenAsync(string clientId,
            string secret,
            string tenantId,
            string authContextPrefixUrl = "https://login.windows.net/",
            string resourceUrl = "https://management.azure.com/")
        {
            try
            {
                string authContextURL = authContextPrefixUrl + tenantId;

                ConfidentialClientApplicationBuilder confidentialClientApplicationBuilder = ConfidentialClientApplicationBuilder.Create(clientId)
                                                                                                       .WithTenantId(tenantId)
                                                                                                       .WithAuthority(authContextURL)
                                                                                                       .WithClientSecret(secret);
                string[] scopes = new[] { $"{resourceUrl}/.default" };
                AuthenticationResult authenticationResult = await confidentialClientApplicationBuilder.Build().AcquireTokenForClient(scopes)
                        .WithSendX5C(true)
                        .ExecuteAsync(CancellationToken.None);

                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetAccessToken(string clientId,
            X509Certificate2 certificate,
            string tenantId,
            string authContextPrefixUrl = "https://login.windows.net/",
            string resourceUrl = "https://management.azure.com/")
        {
            try
            {
                string authContextURL = authContextPrefixUrl + tenantId;

                var tokenCredential = new ClientCertificateCredential(tenantId, clientId, certificate);

                ConfidentialClientApplicationBuilder confidentialClientApplicationBuilder = ConfidentialClientApplicationBuilder.Create(clientId)
                                                                                                       .WithTenantId(tenantId)
                                                                                                       .WithAuthority(authContextURL)
                                                                                                       .WithCertificate(certificate);

                string[] scopes = new[] { $"{resourceUrl}/.default" };
                AuthenticationResult authenticationResult = await confidentialClientApplicationBuilder.Build().AcquireTokenForClient(scopes)
                        .WithSendX5C(true)
                        .ExecuteAsync(CancellationToken.None);

                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static X509Certificate2 GetFunctionalTestCertFromLocalStore(string certificateSubjectName)
        {
            X509Store store = new(StoreName.My, StoreLocation.LocalMachine);

            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                return store.Certificates
                    .Find(X509FindType.FindBySubjectName, certificateSubjectName, validOnly: false)
                    .OfType<X509Certificate2>()
                    .FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}
