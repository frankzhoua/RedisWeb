using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;

namespace redis.WebAPi.Service.CreationFunction
{
    public class StorageAccountCreation
    {
        public static async Task<string> CreateStorageAccountAsync(string region, ResourceGroupResource resourceGroupResource)
        {
            Random random = new Random();
            int randomNumber = random.Next(10, 100); 
            string StorageAccountName = "manual" + region.Replace(" ", "").ToLower() + randomNumber;

            // Get storage account connection string
            StorageAccountResource accountResource = await StorageClient.GetOrCreateStorageAccountAsync(resourceGroupResource,
                    StorageAccountName,
                    region);

            Console.WriteLine($"Successfully created storage account with name '{accountResource.Data.Name}'.");
            var connectionString = StorageClient.GetConnectionStringAsync(accountResource).Result;
            return connectionString;
        }
    }
}
