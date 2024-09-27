using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;

namespace redis.WebAPi.Service.IService
{
    public interface ISubscriptionResourceService
    {
       SubscriptionResource GetSubscription();

        void SetSubscriptionResource(string subscriptionId);
    }
}
