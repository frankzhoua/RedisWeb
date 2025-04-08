using Azure.ResourceManager.Network;
using Azure.ResourceManager.Redis.Models;
using Azure.ResourceManager.RedisEnterprise;
using Azure.ResourceManager.RedisEnterprise.Models;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Model;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.CreationFunction;
using redis.WebAPi.Service.IService;

namespace redis.WebAPi.Service
{
    public class CreationService : ICreationService
    {
        private readonly IRedisCollection _redisCollection;
        private readonly ISubscriptionResourceService _subscriptionResourceService;
        private static Random random = new Random();

        public CreationService(
            IRedisCollection redisCollection,
            ISubscriptionResourceService subscriptionResourceService)
        {
            _redisCollection = redisCollection;
            _subscriptionResourceService = subscriptionResourceService;
        }

        public async Task CreateCache(RedisRequestModel redisReques)
        {
            

            var opt = new RedisOption()
            {
                SkuName = "Premium",
                RegionName = "West US 2",
                NonSSL = true,
            };
            _redisCollection.CreateCache(redisReques.name, opt, redisReques.group);
        }

        public async Task CreateManualCache(RedisRequestModel redisReques)
        {

            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);
            if (redisReques.Cases == null || redisReques.Cases.Length == 0)
            {
                throw new InvalidOperationException("Cases cannot be null or empty.");
            }

            if (redisReques.Quantity != null && redisReques.Cases.Length == 1)
            {
                string caseToCopy = redisReques.Cases[0]; // Get the case to be copied
                int quantity = int.Parse(redisReques.Quantity.ToString()); // to an integer
                redisReques.Cases = Enumerable.Repeat(caseToCopy, quantity).ToArray(); // Copy the specified quantity and replace the original array
            }

            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);
            ResourceGroupResource resourceGroupResource = await _subscriptionResourceService.GetSubscription().GetResourceGroupAsync(redisReques.group);
            var connectioncuse = StorageAccountCreation.CreateStorageAccountAsync("Central US EUAP", resourceGroupResource).Result;
            var connectioneus2e = StorageAccountCreation.CreateStorageAccountAsync("East US 2 EUAP", resourceGroupResource).Result;

            VirtualNetworkResource virtualNetworkcuse = VirtualNetworkCreation.CreateVirtualNetWorkResourceAsync("Central US EUAP", resourceGroupResource).Result;
            VirtualNetworkResource virtualNetworkeus2e = VirtualNetworkCreation.CreateVirtualNetWorkResourceAsync("East US 2 EUAP", resourceGroupResource).Result;


            foreach (var currentTestCase in redisReques.Cases)
            {
                string testCaseKey = currentTestCase.Split(':')[0].Trim();
                string currentDate = DateTime.Now.ToString("MMdd");
                int randomNumber = GenerateFourDigitRandomNumber();

                RedisOption opt = new RedisOption
                {
                    redisCommonConfiguration = new RedisCommonConfiguration()
                };
                var entOpt = new EnterpriseCacheOptions
                {
                    SkuName = RedisEnterpriseSkuName.EnterpriseE10,
                };

                switch (testCaseKey)
                {
                    case "15318659": 
                        //Create E10 Enterprise cache
                        var redisEnterpriseCC = resourceGroupResource.GetRedisEnterpriseClusters();
                        string cacheNameEnt1 = "EnterpriseCache" + "-8659" + "-CUSE-" + currentDate + "-"+ randomNumber;
                        string cacheNameEnt2 = "EnterpriseCache" + "-8659" + "-EUS2E-" + currentDate + "-" + randomNumber;
                        redisReques.region = "Central US EUAP";
                        Task<RedisEnterpriseClusterResource> enterpriseCacheTask1 = EnterpriseCreation.CreateRedisEnterpriseCacheAsync(new P0P1RequestModel(redisReques), entOpt, cacheNameEnt1, redisEnterpriseCC);

                        redisReques.region = "East US 2 EUAP";
                        Task<RedisEnterpriseClusterResource> enterpriseCacheTask2 = EnterpriseCreation.CreateRedisEnterpriseCacheAsync(new P0P1RequestModel(redisReques), entOpt, cacheNameEnt2, redisEnterpriseCC);

                        break;

                    case "15318673":
                        // Create a P1 cache with 2 availability zone
                        // Create a P1 cache with 3 availability zone and 2 replica
                        opt.SkuName = "Premium";
                        opt.RegionName = "East US 2 EUAP";
                        opt.NonSSL = true;
                        opt.Zones = new List<string>{"1","2"};
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-EUS2E-{currentDate}-1", opt, redisReques.group);                        //another option
                        opt.Zones = new List<string> { "1", "2", "3" };
                        opt.ReplicasPerPrimary = 2;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-EUS2E-{currentDate}-2", opt, redisReques.group);                        

                        continue; // Continue the loop to avoid duplicate creation

                    case "15318674":
                        //Create a P1 cache in a virtual network
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.SubnetId = virtualNetworkcuse.Data.Subnets[0].Id;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.SubnetId = virtualNetworkeus2e.Data.Subnets[0].Id;
                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;
                    case "15318675":
                        // Create P1 cache with non-ssl port enabled
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-CUSE-{randomNumber}", opt, redisReques.group);
                        //another option
                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-EUS2E-{randomNumber}", opt, redisReques.group);
                        continue; // Continue the loop to avoid duplicate creation

                    case "15319070":
                        // Create a clustered cache of the desired configuration
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-CUSE-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-EUS2E-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15319116":
                        // Create a clustered premium P1 cache with public endpoint enabled, redis version 6 and multi replica enabled (replica count = 2, shard count = 2). 
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.Cluster = true;
                        opt.ReplicasPerPrimary = 2;
                        opt.MinShards = 2;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-CUSE-{randomNumber}", opt, redisReques.group);
                        
                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-EUS2E-{randomNumber}", opt, redisReques.group);
                        break;

                    case string key when key == "15320703" || key == "16021106" || key == "16021140 " || key == "16021226 ":
                        // Create two P1 caches one in EUS and another one in SEasia, please prefix the name of these caches with ManualTestingGeo-**.
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-SEA-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15379676":
                        //Create a premium (P1) cache with clustering enabled (two shards)
                        opt.SkuName = "Premium";
                        opt.Cluster = true;
                        opt.MinShards = 2;
                        opt.RegionName = "Central US EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15379764":
                        //Create a clustered cache (P1) within a virtual network with AOF persistence enabled.
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.SubnetId = virtualNetworkcuse.Data.Subnets[0].Id;
                        opt.redisCommonConfiguration.AofStorageConnectionString0 = connectioncuse;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.SubnetId = virtualNetworkeus2e.Data.Subnets[0].Id;
                        opt.redisCommonConfiguration.AofStorageConnectionString0 = connectioneus2e;
                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15379484":
                        //Create a premium (P1) cache with default configuration. 
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15379874":
                        //Create a clustered cache (P1) within a virtual network with multi replica (replica count 3)
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.SubnetId = virtualNetworkcuse.Data.Subnets[0].Id;
                        opt.ReplicasPerPrimary = 2;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.SubnetId = virtualNetworkeus2e.Data.Subnets[0].Id;
                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "15380305":
                        //Create two premium clustered (shardCount=2) caches in different regions with identical configurations (P1) with public endpoint enabled.
                        opt.SkuName = "Premium";
                        opt.Cluster = true;
                        opt.MinShards = 2;
                        opt.RegionName = "Central US EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    case "21253924":
                        //Create a premium (P1) cache with default configuration. 
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-CUSE-{currentDate}-{randomNumber}", opt, redisReques.group);

                        opt.RegionName = "East US 2 EUAP";
                        _redisCollection.CreateCache($"ManualTestingGeo-{testCaseKey}-EUS2E-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;

                    default:
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        _redisCollection.CreateCache($"ManualTest-{testCaseKey}-{currentDate}-{randomNumber}", opt, redisReques.group);
                        break;
                }

                     
            }

            
        }

        public async Task CreateBVTCache(RedisRequestModel redisReques)
        {
            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);

            for (int i = 0; i < BVTTestCaseName.BVTTestCaseNames.Length; i++)
            {

                string currentTestCase = BVTTestCaseName.BVTTestCaseNames[i];
                string currentDate = DateTime.Now.ToString("MMdd");

                RedisOption opt = new RedisOption();

                if (currentTestCase == "RebootBladeTest")
                {
                    opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = "Central US EUAP",
                        Cluster = true,
                        MaxShards = 2,
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-" + currentDate, opt, redisReques.group);
                }
                else if (currentTestCase == "DataPersistenceBladeTest-NotPremium")
                {
                    opt = new RedisOption()
                    {
                        SkuName = "Basic",
                        RegionName = "Central US EUAP",
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-" + currentDate, opt, redisReques.group);
                }
                else if (currentTestCase == "GeoreplicationBladeTest")
                {
                    opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = "Central US EUAP",
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-CUSE-" + currentDate, opt, redisReques.group);
                    opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = "East US",
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-EUS-" + currentDate, opt, redisReques.group);
                }
                else
                {
                    opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = "Central US EUAP",
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-" + currentDate, opt, redisReques.group);
                }
            }
        }

        public async Task CreateBVTCacheByCase(RedisRequestModel redisReques)
        {
            if (redisReques.Cases == null || redisReques.Cases.Length == 0)
            {
                throw new InvalidOperationException("Cases cannot be null or empty.");
            }

            if (redisReques.Quantity != null && redisReques.Cases.Length == 1)
            {
                string caseToCopy = redisReques.Cases[0]; // Get the case to be copied
                int quantity = int.Parse(redisReques.Quantity.ToString()); // to an integer
                redisReques.Cases = Enumerable.Repeat(caseToCopy, quantity).ToArray(); // Copy the specified quantity and replace the original array
            }

            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);

            foreach (var currentTestCase in redisReques.Cases)
            {
                string currentDate = DateTime.Now.ToString("MMdd");
                int randomNumber = GenerateFourDigitRandomNumber();

                RedisOption opt = new RedisOption();

                switch (currentTestCase)
                {
                    case "RebootBladeTest":
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.Cluster = true;
                        opt.MaxShards = 2;
                        opt.NonSSL = true;
                        break;

                    case "DataPersistenceBladeTest-NotPremium":
                        opt.SkuName = "Basic";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        break;

                    case "GeoreplicationBladeTest":
                        // Create two caching options
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        _redisCollection.CreateCache($"BVT-{currentTestCase}-{currentDate}-CUSE-{randomNumber}", opt, redisReques.group);
                        //another option
                        opt.RegionName = "East US";
                        _redisCollection.CreateCache($"BVT-{currentTestCase}-{currentDate}-EUS-{randomNumber}", opt, redisReques.group);
                        continue; // Continue the loop to avoid duplicate creation

                    default:
                        opt.SkuName = "Premium";
                        opt.RegionName = "Central US EUAP";
                        opt.NonSSL = true;
                        break;
                }

                _redisCollection.CreateCache($"BVT-{currentTestCase}-{currentDate}-{randomNumber}", opt, redisReques.group);
            }

        }

        public async Task CreatePerfCache(PerfRequestModel redisReques)
        {
            string RegionName_1 = "East US 2 EUAP";
            string formattedDate = DateTime.Now.ToString("MMdd");

            _subscriptionResourceService.SetSubscriptionResource(redisReques.Subscription);
            if (redisReques.Sku == "All" || redisReques.Sku == "Premium")
            {
                for (int i = 1; i <= 5; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,
                    };

                    _redisCollection.CreateCache("Verifyperformance-P" + i + "-EUS2E" + "-" + formattedDate, opt, redisReques.Group);
                }
            }
            if (redisReques.Sku == "All" || redisReques.Sku == "Standard")
            {
                for (int i = 0; i <= 6; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Standard",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,

                    };
                    _redisCollection.CreateCache("Verifyperformance-C" + i + "-EUS2E-Standard" + "-" + formattedDate, opt, redisReques.Group);
                }
            }
            if (redisReques.Sku == "All" || redisReques.Sku == "Basic")
            {
                for (int i = 0; i <= 6; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Basic",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("Verifyperformance-C" + i + "-EUS2E-Basic" + "-" + formattedDate, opt, redisReques.Group);

                }
            }
            // Processing received parameters
        }

        public async Task CreateAltCache(PerfRequestModel redisReques)
        {
            string RegionName_1 = "East US 2 EUAP";
            string formattedDate = DateTime.Now.ToString("MMdd");

            _subscriptionResourceService.SetSubscriptionResource(redisReques.Subscription);
            if (redisReques.Sku == "All" || redisReques.Sku == "Premium")
            {
                for (int i = 1; i <= 5; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Premium",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("Alt-eus2e-P" + i + "-" + formattedDate, opt, redisReques.Group);
                }
            }
            if (redisReques.Sku == "All" || redisReques.Sku == "Standard")
            {
                for (int i = 0; i <= 6; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Standard",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("Alt-eus2e-SC" + i + "-" + formattedDate, opt, redisReques.Group);
                }
            }
            if (redisReques.Sku == "All" || redisReques.Sku == "Basic")
            {
                for (int i = 0; i <= 6; i++)
                {
                    RedisOption opt = new RedisOption()
                    {
                        SkuName = "Basic",
                        RegionName = RegionName_1,
                        SkuCapacity = i,
                        NonSSL = true,
                    };
                    _redisCollection.CreateCache("Alt-eus2e-BC" + i + "-" + formattedDate, opt, redisReques.Group);

                }
            }
            // Processing received parameters
        }

        public async Task CreateP0P1Cache(P0P1RequestModel redisReques)
        {
            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);

            ResourceGroupResource resourceGroupResource = await _subscriptionResourceService.GetSubscription().GetResourceGroupAsync(redisReques.group);

            var connectionString = StorageAccountCreation.CreateStorageAccountAsync(redisReques.region, resourceGroupResource).Result;

            VirtualNetworkResource virtualNetwork = VirtualNetworkCreation.CreateVirtualNetWorkResourceAsync(redisReques.region, resourceGroupResource).Result;

            string currentDate = DateTime.Now.ToString("MMdd");

            List<Task> tasks = new List<Task>();
            foreach (var opt in P0P1ManualCacheModel.opt)
            {
                tasks.Add(Task.Run(() =>
                {
                    if (redisReques.region == "Central US EUAP" && opt.Zones != null)
                    {
                        return;
                    }

                    bool ifPrivateLink = false;
                    bool ifGeo = false;
                    foreach (var tag in opt.Tag)
                    {
                        if (tag == "AOF")
                        {
                            opt.redisCommonConfiguration.IsAofBackupEnabled = true;
                            opt.redisCommonConfiguration.AofStorageConnectionString0 = connectionString;
                        }
                        if (tag == "Vnet")
                        {
                            opt.SubnetId = virtualNetwork.Data.Subnets[0].Id;
                        }
                        if (tag == "PrivateLink")
                        {
                            ifPrivateLink = true;
                        }
                        if (tag == "Geo")
                        {
                            ifGeo = true;
                        }
                    }
                    opt.RegionName = redisReques.region;
                    string cacheName = "ManualTest-" + opt.Tag[0] + redisReques.region.Replace(" ", "") + currentDate;
                    _redisCollection.CreateCache(cacheName, opt, redisReques.group);

                    string cacheName2;
                    if (ifGeo == true && redisReques.region == "Central US EUAP")
                    {
                        opt.RegionName = "East US";
                        cacheName2 = "ManualTest-" + opt.Tag[0] + redisReques.region.Replace(" ", "") + currentDate;
                        _redisCollection.CreateCache(cacheName2, opt, redisReques.group);
                    }
                    else
                    {
                        opt.RegionName = "Southeast Asia";
                        cacheName2 = "ManualTest-" + opt.Tag[0] + redisReques.region.Replace(" ", "") + currentDate;
                        _redisCollection.CreateCache(cacheName2, opt, redisReques.group);
                    }
                }));
            }
            var redisEnterpriseCC = resourceGroupResource.GetRedisEnterpriseClusters();

            List<Task> tasks1 = new List<Task>();
            foreach (var entOpt in P0P1ManualCacheModel.EntOpt)
            {
                tasks1.Add(Task.Run(async () =>
                {
                    string cacheNameEnt = entOpt.Tag[0] + redisReques.region.Replace(" ", "") + currentDate;
                    RedisEnterpriseClusterResource EnterpriseCache = await EnterpriseCreation.CreateRedisEnterpriseCacheAsync(redisReques, entOpt,
                    cacheNameEnt,
                    redisEnterpriseCC);
                }));
            }
        }

        private static int GenerateFourDigitRandomNumber()
        {
            return random.Next(1000, 10000);
        }

        
    }
}
