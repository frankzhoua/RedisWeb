using Azure.Core;
using Azure.ResourceManager;

namespace redis.WebAPi.Service.IService
{
    public interface IAzureClient
    {
        public ArmClient InitializeAzureClientAsync();
        public void SetSub(ArmClient arm, string sub);
    }
}
