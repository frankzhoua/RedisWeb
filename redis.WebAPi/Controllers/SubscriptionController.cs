using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;
namespace redis.WebAPi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {

        private readonly ISubscriptionResourceService _subscriptionResourceService;

        // 注入 SubscriptionResourceService
        public SubscriptionController(ISubscriptionResourceService subscriptionResourceService)
        {
            _subscriptionResourceService = subscriptionResourceService;
        }

        // 接收前端传递的 subscriptionId 并生成 SubscriptionResource
        [HttpGet]
        [HttpGet("{subscriptionId}")]
        public IActionResult SetSubscription(string subscriptionId)
        {
            try
            {
                // 根据前端传入的 subscriptionId 生成 SubscriptionResource
                _subscriptionResourceService.SetSubscriptionResource(subscriptionId);
                List<string> groupNameList = new List<string>();
                foreach (var s in _subscriptionResourceService.GetSubscription().GetResourceGroups().GetAll())
                {
                    groupNameList.Add(s.Data.Name);
                }

                return Ok(groupNameList.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error setting subscription: {ex.Message}");
            }
        }

        // 示例：获取 SubscriptionResource 并进行某种操作
        [HttpGet("getSubscriptionResource")]
        public IActionResult GetSubscriptionResource()
        {
            try
            {
                var subscriptionResource = _subscriptionResourceService.GetSubscription();
                return Ok(subscriptionResource.Data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving subscription: {ex.Message}");
            }
        }
    }
}
