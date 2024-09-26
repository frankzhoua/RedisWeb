using Azure.Identity;
using Azure;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System.Reflection.Metadata.Ecma335;

namespace redis.WebAPi.Service.AzureShared
{
    public interface IRedisFunctionalTestKeyVaultObjectsRetriever
    {
        Task<X509Certificate2> GetCertificateWithCredentialAsync(string certificateName, TokenCredential tokenCredential);

        Task<X509Certificate2> GetCertificateWithThumbprintAsync(string certificateName, string cloudTestClientId, string certThumbprint);

        Task<string> GetSecretWithCredentialAsync(string secretName, TokenCredential tokenCredential);

        Task<string> GetSecretWithThumbprintAsync(string secretName, string cloudTestClientId, string certThumbprint);
    }

    public class RedisFunctionalTestKeyVaultObjectsRetriever : IRedisFunctionalTestKeyVaultObjectsRetriever
    {

        private readonly string _tenantId;
        private readonly Uri _clientKeyVaultUri;

        public RedisFunctionalTestKeyVaultObjectsRetriever(string tenantId, Uri clientKeyVaultUri)
        {
            _tenantId = tenantId;
            _clientKeyVaultUri = clientKeyVaultUri;
        }

        public async Task<X509Certificate2> GetCertificateWithCredentialAsync(string certificateName, TokenCredential tokenCredential)
        {
            var secretClient = new SecretClient(_clientKeyVaultUri, tokenCredential);

            Response<KeyVaultSecret> kvSecret = await secretClient.GetSecretAsync(certificateName);

            var certBytes = Convert.FromBase64String(kvSecret.Value.Value);

            return new X509Certificate2(certBytes);
        }

        public async Task<X509Certificate2> GetCertificateWithThumbprintAsync(string certificateName, string cloudTestClientId, string certThumbprint)
        {
            var secretClient = new SecretClient(_clientKeyVaultUri,
                new ClientCertificateCredential(_tenantId, cloudTestClientId, GetCertificateFromStore(certThumbprint)));

            Response<KeyVaultSecret> kvSecret = await secretClient.GetSecretAsync(certificateName);

            var certBytes = Convert.FromBase64String(kvSecret.Value.Value);

            return new X509Certificate2(certBytes);
        }

        public async Task<string> GetSecretWithCredentialAsync(string secretName, TokenCredential tokenCredential)
        {
            var secretClient = new SecretClient(_clientKeyVaultUri, tokenCredential);
            Response<KeyVaultSecret> kvSecret = await secretClient.GetSecretAsync(secretName);
            return kvSecret.Value.Value;
        }

        public async Task<string> GetSecretWithThumbprintAsync(string secretName, string cloudTestClientId, string certThumbprint)
        {
            var secretClient = new SecretClient(_clientKeyVaultUri,
                new ClientCertificateCredential(_tenantId, cloudTestClientId, GetCertificateFromStore(certThumbprint)));

            Response<KeyVaultSecret> kvSecret = await secretClient.GetSecretAsync(secretName);
            return kvSecret.Value.Value;
        }

        private X509Certificate2 GetCertificateFromStore(string thumbprint)
        {
            X509Store store = new(StoreName.My, StoreLocation.LocalMachine);

            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                return store.Certificates
                    .Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false)
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
