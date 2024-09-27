using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;
using redis.WebAPi.Service.IService;

namespace redis.WebAPi.Service.AzureShared
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

        // 获取 SubscriptionResource
        public SubscriptionResource GetSubscriptionResource()
        {
            if (_subscriptionResource == null)
            {
                throw new InvalidOperationException("SubscriptionResource has not been set. Please provide a subscriptionId.");
            }
            return _subscriptionResource;
        }

    }
}
