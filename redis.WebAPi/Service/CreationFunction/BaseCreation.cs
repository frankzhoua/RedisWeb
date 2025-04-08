using Azure.ResourceManager.Redis.Models;
using Azure.ResourceManager.Redis;
using Azure;
using Azure.Core;
using redis.WebAPi.Service.AzureShared;

namespace redis.WebAPi.Service.CreationFunction
{
    public class BaseCreation
    {
        public static async Task<RedisResource> CreateRedisResource(string CacheName, RedisOption options,
    RedisCollection RedisCollection, bool forceCreateReplicas = false)
        {
            // TODO: Handle the caches in provision failed state

            var cacheName = CacheName;

            RedisCreateOrUpdateContent createParams = CreateParametersFromOptions(options, forceCreateReplicas);

            RedisResource cache = (await RedisCollection.CreateOrUpdateAsync(WaitUntil.Completed, cacheName, createParams)).Value;

            Console.WriteLine("Successfully created" + cache.Data.Name);
            return cache;
        }

        private static RedisCreateOrUpdateContent CreateParametersFromOptions(RedisOption options, bool forceCreateReplicas = false)
        {
            RedisSku? skuobj = null;
            int skucapacity = 1;
            if (options.SkuCapacity > 1)
            {
                skucapacity = options.SkuCapacity;
            }

            switch (options.SkuName.ToString().ToLower())
            {
                case "basic":
                    skuobj = new RedisSku(new RedisSkuName("Basic"), new RedisSkuFamily("C"), skucapacity);
                    break;

                case "standard":
                    skuobj = new RedisSku(new RedisSkuName("Standard"), new RedisSkuFamily("C"), skucapacity);
                    break;

                case "premium":
                    skuobj = new RedisSku(new RedisSkuName("Premium"), new RedisSkuFamily("P"), skucapacity);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported sku: {options.SkuName}");
            }

            var location = new AzureLocation(options.RegionName);
            //Identity need re

            var parameters = new RedisCreateOrUpdateContent(location, skuobj)
            {

                SubnetId = options.SubnetId,
                EnableNonSslPort = options.NonSSL.Equals(true) && options.NonSSL,
                RedisConfiguration = new RedisCommonConfiguration()

            };
            if (options.redisCommonConfiguration != null)
            {
                parameters.RedisConfiguration = options.redisCommonConfiguration;
            }

            //parameters.RedisConfiguration.AdditionalProperties.Add("CacheVmType", BinaryData.FromString("\"IaaS\""));
            parameters.RedisConfiguration.AdditionalProperties["CacheVmType"] = BinaryData.FromString("\"IaaS\"");


            if (skuobj.Name.ToString() == "Premium" && options.MinShards.HasValue && options.Cluster.HasValue && options.Cluster.Value)
            {
                parameters.ShardCount = options.MinShards.Value;
            }

            if (options.Zones != null)
            {
                foreach (var s in options.Zones)
                {
                    parameters.Zones.Add(s.ToString());
                }
            }

            if (options.RedisVersion != null)
            {
                parameters.RedisVersion = options.RedisVersion;
            }
            // If RedisVersion is not specified, default will be used

            if (forceCreateReplicas || options.ReplicasPerPrimary.HasValue && skuobj.Name.ToString() == "Premium")
            {
                parameters.ReplicasPerMaster = options.ReplicasPerPrimary;
                parameters.ReplicasPerPrimary = options.ReplicasPerPrimary;
            }

            return parameters;
        }
    }
}
