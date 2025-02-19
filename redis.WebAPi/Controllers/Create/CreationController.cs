using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Service.IService;
using redis.WebAPi.Filters;
using redis.WebAPi.Models;
using redis.WebAPi.Model;

namespace redis.WebAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AuthFilter))]
    public class CreationController : ControllerBase
    {
        private readonly ICreationService _creationService;

        public CreationController( ICreationService creationService)
        {
            _creationService = creationService;   
        }

        [HttpPost()]
        public async Task<IActionResult> CreateCache([FromBody] RedisRequestModel redisRequest)
        {
            await _creationService.CreateCache(redisRequest);
            return Ok();
        }

        [HttpPost("CreateManualCache")]
        public async Task<IActionResult> CreateManualCache([FromBody] RedisRequestModel redisRequest)
        {
            await _creationService.CreateManualCache(redisRequest);
            return Ok();
        }

        [HttpPost("CreateBVTCache")]
        public async Task<IActionResult> CreateBVTCache([FromBody] RedisRequestModel redisRequest)
        {
            await _creationService.CreateBVTCache(redisRequest);
            return Ok();
        }

        [HttpPost("CreateBVTCacheByCase")]
        public async Task<IActionResult> CreateBVTCacheByCase([FromBody] RedisRequestModel redisRequest)
        {
            await _creationService.CreateBVTCacheByCase(redisRequest);
            return Ok();
        }

        [HttpPost("CreatePerfCache")]
        public async Task<IActionResult> CreatePerfCache([FromBody] PerfRequestModel redisRequest)
        {
            await _creationService.CreatePerfCache(redisRequest);
            return Ok();
        }

        [HttpPost("CreateAltCache")]
        public async Task<IActionResult> CreateAltCache([FromBody] PerfRequestModel redisRequest)
        {
            await _creationService.CreateAltCache(redisRequest);
            return Ok();
        }

        [HttpPost("CreateP0P1Cache")]
        public async Task<IActionResult> CreateP0P1Cache([FromBody] P0P1RequestModel redisRequest)
        {
            await _creationService.CreateP0P1Cache(redisRequest);
            return Ok();
        }

        //[HttpPost("VMConnection")]
        //public async Task<IActionResult> VMConnection()
        //{
        //    return Ok(await _connectionVMService.ConnectionVM());

        //}
    }
}
