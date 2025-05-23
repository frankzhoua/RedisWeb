﻿using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Filters;
using redis.WebAPi.Model;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;
namespace redis.WebAPi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AuthFilter))]
    public class StackExchangeController : ControllerBase
    {

        private readonly ISubscriptionResourceService _subscriptionResourceService;
        private readonly IStackExchangeService _stackExchangeService;

        // Injecting SubscriptionResourceService
        public StackExchangeController(ISubscriptionResourceService subscriptionResourceService, IStackExchangeService stackExchangeService)
        {
            _subscriptionResourceService = subscriptionResourceService;
            this._stackExchangeService = stackExchangeService;
        }
        //Add data interface
        [HttpPost("AddDataToRedis")]
        public async Task<IActionResult> AddDataToRedis([FromBody] StackExchangeModel redisRequest)
        {
            try
            {
                _subscriptionResourceService.SetSubscriptionResource(redisRequest.Subscription);
                _stackExchangeService.AddData(redisRequest.Group, redisRequest.Name, redisRequest.Port, redisRequest.Ssl, redisRequest.NumKeysPerShard);
                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                // Handle argument null exception
                return BadRequest("Request parameters cannot be null.");
            }
            catch (InvalidOperationException ex)
            {
                // Handle invalid operation exception
                return StatusCode(500, "Internal server error.");
            }
            catch (Exception ex)
            {
                // Handle all other types of exceptions
                return StatusCode(500, "An unknown error occurred.");
            }
        }
    }
}
