using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.ManagedServiceIdentities;
using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;
using redis.WebAPi.Service.IService;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace redis.WebAPi.Service.AzureShared
{
    /// <summary>
    /// Client to communicate with Azure
    /// </summary>
    public class AzureClientFactory
    {
        public ArmClient ArmClient { get; set; }

        public AzureClientFactory() { ArmClient = InitializeAzureClientAsync(); }

        public ArmClient InitializeAzureClientAsync()
        {
            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

            string? applicationId = null;

            // For Prod, TiP and Stage environments where the tenantId is known, load up known 
            // known applicationId
            applicationId = "d12243c1-a5bb-4217-92ba-c0da3d5c10ab";

            ArmClient? armClient = null;

            TokenCredential? tokenCredential = null;

            // For instances where application id is not loaded, like, executing on local dev box in outlook based tenant
            // for dogfood/stage environments, use powershell context. You can login using powershell 
            // 'Login-AzAccount -Environment Dogfood'
            tokenCredential = new AzurePowerShellCredential();

            // TODO: Find on setting login authhost from visualstudio, when set, we can use defaultcredential instead
            // of powershell context
            //VisualStudioCredential visualStudioCredential = new VisualStudioCredential(new VisualStudioCredentialOptions()
            //{
            //    AuthorityHost = EnvironmentConstants.GetAuthorityHost(cloud),
            //});
            armClient = new ArmClient(tokenCredential);

            armClient ??= GetArmClientAuthBasedOnApp(tenantId, applicationId, armClient).Result;
            var sub = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "1e57c478-0901-4c02-8d35-49db234b78d2")); 
           
            return armClient;
        }

        public string SetSub(ArmClient arm, string sub) 
        {
            var subResource = arm.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "1e57c478-0901-4c02-8d35-49db234b78d2"));
            var resource = subResource.GetResourceGroup("Test-MaJunRu").Value;
            var redis = resource.GetRedis("BVT-cuse-sc0-0926").Value;
            return redis.Data.Name.ToString();

        }

        /// <summary>
        /// For running tests in stage environment using Cloud test pipelines, we need to use certificate based auth
        /// Cloud Test VM -> Fetch certificate from local store -> Get Access to CloudTestApp
        /// -> Using CloudTestApp, get access to Team Secrets Keyvault -> Fetch certificate from Team Secrets Keyvault
        /// -> Use certificate to get access to stage FunctionalTestsApp (present in different tenant) -> Use stage FunctionalTestsApp
        /// to get access to stage subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="tenantId"></param>
        /// <param name="applicationId"></param>
        /// <param name="armClient"></param>
        /// <returns></returns>


        private static async Task<ArmClient> GetArmClientAuthBasedOnApp(string? tenantId, string? applicationId, ArmClient? armClient)
        {
            Uri authHost = AzureAuthorityHosts.AzurePublicCloud;
            Uri keyVaultUril = new Uri("https://azurecache-sharedsecrets.vault.azure.net/");

            string certificateName = "RedisCacheFunctionalTests";
            X509Certificate2 functionalTestsCertificate = await AuthHelpers.GetCertificateFromKeyVaultAsync(certificateName, keyVaultUril, tenantId!);

            ClientCertificateCredential clientCertificateCredential = new ClientCertificateCredential(tenantId,
                applicationId,
                functionalTestsCertificate,
                new ClientCertificateCredentialOptions()
                {
                    AuthorityHost = authHost,
                    SendCertificateChain = true
                });

            armClient = new ArmClient(clientCertificateCredential);

            return armClient;
        }

        private static async Task<List<RedisResource>> GetAllRedisResourcesAsync(RedisCollection redisCollection)
        {
            var redisList = new List<RedisResource>();
            var redisResources = redisCollection?.GetAllAsync();
            if (redisResources != null)
            {
                await foreach (var redis in redisResources)
                {
                    redisList.Add(redis);
                }
            }
            return redisList;
        }

    }
}
