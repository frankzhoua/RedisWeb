using Azure.ResourceManager.Redis;

namespace redis.WebAPi.Service.IService
{
    public interface IRedisCollection
    {
        RedisCollection GetRedisCollection(ISubscriptionResourceService subR, string groupName);

    }
}
