using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Redis;
using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Filters;
using redis.WebAPi.Service.AzureShared;
using redis.WebAPi.Service.IService;

namespace redis.WebAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AuthFilter))]
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
            // Use the factory to generate an ArmClient instance
            var armClient = _client.ArmClient;
            var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "1e57c478-0901-4c02-8d35-49db234b78d2"));
            var resourceGroup = (await subResource.GetResourceGroupAsync(resourceGroupName)).Value;


            if (resourceGroup is null)
            {
                return NotFound();
            }

            return Ok(resourceGroup);
        }

        [HttpDelete("DeleteResource")]
        public async Task<IActionResult> DeleteResource(string subscription, string resourceGroupName)
        {
            try
            {
                var armClient = _client.ArmClient;
                var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + subscription));
                var resourceGroupResource = subResource.GetResourceGroupAsync(resourceGroupName).Result.Value.GetGenericResources().ToList();

                for (var i = 0; i < resourceGroupResource.Count; i++)
                {
                    await resourceGroupResource[i].DeleteAsync(WaitUntil.Started);
                }

                return Ok("Delete" + resourceGroupName + "Successfully");
            }
            catch (Exception ex)
            {
                // Record abnormal information or perform other processing
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting resources.");
            }
        }

    }
}
