using Azure.ResourceManager;
using Azure.ResourceManager.Redis;
using redis.WebAPi.Service.IService;
using Azure.ResourceManager.Resources;

namespace redis.WebAPi.Service
{
    public class RedisCollectionService : IRedisCollection
    {
        public RedisCollection redisResources { get; set; }

        public RedisCollectionService() { }

        public RedisCollection GetRedisCollection(ISubscriptionResourceService subR, string groupName)
        {
            var redisCollection = subR.GetSubscription().GetResourceGroup(groupName).Value.GetAllRedis();
            return redisCollection;

        }

    }
}
