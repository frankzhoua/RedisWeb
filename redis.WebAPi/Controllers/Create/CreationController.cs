﻿using Azure.Core;
using Azure.ResourceManager;
using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Model;
using redis.WebAPi.Service;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.AzureShared.CreationFunction;
using redis.WebAPi.Service.IService;
using System.Data;
using System;

namespace redis.WebAPi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CreationController : ControllerBase
    {
        private readonly IRedisCollection _redisCollection;
        private readonly ISubscriptionResourceService _subscriptionResourceService;

        private static Random random = new Random();

        public CreationController(IRedisCollection redisCollection, ISubscriptionResourceService subscriptionResourceService)
        {
            this._redisCollection = redisCollection;
            this._subscriptionResourceService = subscriptionResourceService;
        }


        [HttpPost()]
        public async Task<IActionResult> CreateCache([FromBody] RedisRequestModel redisReques)
        {
            RedisOption opt = new RedisOption() 
            {
                SkuName = "Premium",
                RegionName = "West US 2",
                NonSSL = true,
            };

            _redisCollection.CreateCache(redisReques.name, opt, redisReques.group);
            return Ok();
        }

        [HttpPost("CreateBVTCache")]
        //Currently, PrivateEndpointBladeTest, CacheCreationTest, and EnterpriseTest 
        //need to be manually verified and created, and are not included in this feature.
        public async Task<IActionResult> CreateBVTCache([FromBody] RedisRequestModel redisReques)
        {

            _subscriptionResourceService.SetSubscriptionResource(redisReques.subscription);

            for (int i = 0; i < BVTTestCaseName.BVTTestCaseNames.Length; i++) {
                
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
                    _redisCollection.CreateCache("BVT-" + currentTestCase + "-CUSE-"+ currentDate, opt, redisReques.group);
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
            return Ok();
        }

        [HttpPost("CreateBVTCacheByCase")]
        //Currently, PrivateEndpointBladeTest, CacheCreationTest, and EnterpriseTest 
        //need to be manually verified and created, and are not included in this feature.
        public async Task<IActionResult> CreateBVTCacheByCase([FromBody] RedisRequestModel redisReques)
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

            return Ok();
        }
        private static int GenerateFourDigitRandomNumber()
        {
            return random.Next(1000, 10000); // Generate random numbers between 1000 and 9999
        }

    }
}
