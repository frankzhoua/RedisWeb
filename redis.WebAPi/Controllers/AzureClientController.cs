using Azure.Core;
using Azure.ResourceManager;
using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;

namespace youjun.NETDemo.WebAPi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureClientController : ControllerBase
    {
        private readonly AzureClientFactory _client;

        public AzureClientController(AzureClientFactory azure) 
        {
            this._client = azure;
        }
        
        [HttpGet("{resourceGroupName}")]
        public async Task<IActionResult> GetResourceGroup(string resourceGroupName)
        {
            // 使用工厂生成 ArmClient 实例
            var armClient = _client.ArmClient;
            var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "1e57c478-0901-4c02-8d35-49db234b78d2"));
            var resourceGroup = (await subResource.GetResourceGroupAsync(resourceGroupName)).Value;


            if (resourceGroup is null)
            {
                return NotFound();
            }

            return Ok(resourceGroup);
        }




    }
}
