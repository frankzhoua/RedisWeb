using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;

namespace redis.WebAPi.Service
{
    public class SubscriptionResourceService : ISubscriptionResourceService
    {
        private readonly ArmClient _armClient;
        private SubscriptionResource _subscriptionResource;

        // 构造函数，注入 ArmClient
        public SubscriptionResourceService(AzureClientFactory armClient)
        {
            _armClient = armClient.ArmClient;
        }

        public SubscriptionResource GetSubscription()
        {
            return _subscriptionResource;
        }

        // 根据传入的 subscriptionId 创建 SubscriptionResource
        public void SetSubscriptionResource(string subscriptionId)
        {
            _subscriptionResource = _armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + subscriptionId));
        }


    }
}
