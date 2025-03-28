﻿using Azure.Core;
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

        // Constructor, inject ArmClient
        public SubscriptionResourceService(AzureClientFactory armClient)
        {
            _armClient = armClient.ArmClient;
        }

        public SubscriptionResource GetSubscription()
        {
            if (_subscriptionResource == null)
            {
                throw new InvalidOperationException("Subscription resource is not set.");
            }
            return _subscriptionResource;
        }

        // Create a SubscriptionResource based on the passed subscriptionId
        public void SetSubscriptionResource(string subscriptionId)
        {
            _subscriptionResource = _armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + subscriptionId));
        }

        public RedisCollection GetRedisCollection(SubscriptionResource sub , string group)
        {
            return sub.GetResourceGroup(group).Value.GetAllRedis();
        }

    }
}
