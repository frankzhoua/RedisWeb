using Azure.Core;
using Azure.ResourceManager.Compute.Models;
using Azure;
using Azure.ResourceManager.Compute;
using redis.WebAPi.Service.Benchmark;
using redis.WebAPi.Repository.AppDbContext;
using Microsoft.EntityFrameworkCore;



namespace redis.WebAPi.Service.AzureShared
{
    public class ConnectionVMService 
    {
        private readonly AzureClientFactory _client;
        private readonly InsertBenchmarkService _insertBenchmarkService;
        private readonly BenchmarkContent _dbContext;      

        public ConnectionVMService(BenchmarkContent dbContext, AzureClientFactory client, InsertBenchmarkService insertBenchmarkService)
        {

            _client = client;
            _insertBenchmarkService = insertBenchmarkService;
            _dbContext = dbContext;
            
        }

        public async Task<string> ConnectionVM()
        {
            try
            {
                var benchmarkTask = await _dbContext.BenchmarkQueue
                    .Where(b => b.Status == 2)  // 只处理待处理任务
                    .OrderBy(b => b.TimeStamp)  // 按照时间戳排序，先处理早期任务
                    .FirstOrDefaultAsync();

                if (benchmarkTask == null)
                {
                    return "No pending benchmark tasks found.";
                }

                string cacheName = benchmarkTask.Name;
                string primary = benchmarkTask.pw;
                int clients = benchmarkTask.Clients;
                int threads = benchmarkTask.Threads;
                int size = benchmarkTask.Size;
                int requests = benchmarkTask.Requests;
                int pipeline = benchmarkTask.Pipeline;
                int times = benchmarkTask.Times;

                var vm = await GetVMByCacheName(cacheName);
                if (!await IsVMAvailableForTask(vm))
                {
                    throw new InvalidOperationException("VM is busy with another task.");
                }

                // 运行基准测试
                string output = await RunBenchmarkOnVM(vm, cacheName, primary, clients, threads, size, requests, pipeline, times);

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        // Check whether the VM is idle to ensure that the task execution status is more accurately determined
        private async Task<bool> IsVMAvailableForTask(VirtualMachineResource vm)
        {
            var instanceView = await vm.InstanceViewAsync();
            var statuses = instanceView.Value.Statuses;
        
            bool isRunningAnotherTask = statuses.Any(status => status.Code == "PowerState/running" && status.DisplayStatus.Contains("Running"));
  
            return !isRunningAnotherTask;
        }


        private async Task<string> RunBenchmarkOnVM(VirtualMachineResource vm, string name, string pw, int clients, int threads, int size, int requests, int pipeline, int times)
        {
            var timeStamp = DateTime.Now;
            string fileName = $"output-{timeStamp}";

            var runCommandInput = new RunCommandInput("RunShellScript")
            {
                Script =
            {
                "cd /home/azureuser",
                $"./manage_screen_session.sh {name} {pw} {threads} {clients} {requests} {pipeline} {size} {times} {fileName}",
            }
            };

            var response = (await vm.RunCommandAsync(WaitUntil.Completed, runCommandInput)).Value;
            var output = string.Join("\n", response.Value.Select(r => r.Message));

            if (!string.IsNullOrEmpty(output))
            {
                await _insertBenchmarkService.InsertBenchmarkResultData(output, name, timeStamp);
                var removedData = await _dbContext.BenchmarkQueue.FirstOrDefaultAsync(u => u.Name == name);
                if (removedData != null)
                {
                    _dbContext.BenchmarkQueue.Remove(removedData);
                    await _dbContext.SaveChangesAsync();
                }

            }

            return output;
        }



        public async Task<VirtualMachineResource> GetVirtualMachineAsync(string vmName)
        {
            var armClient = _client.ArmClient;
            var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "fc2f20f5-602a-4ebd-97e6-4fae3f1f6424"));
            var vmResource = (await subResource.GetResourceGroupAsync("MemtierbenchmarkTest")).Value.GetVirtualMachines().GetAsync(vmName).Result;

            return vmResource;
        }

        public async Task<VirtualMachineResource> GetVMByCacheName(string cacheName)
        {
            string vmName = AllocateVMByCacheName(cacheName);
            return await GetVirtualMachineAsync(vmName);
        }

        private string AllocateVMByCacheName(string cacheName)
        {
            cacheName = cacheName.ToLower();



            if (cacheName.Contains("p1") || cacheName.Contains("p2")) return "MemtierBenchmarkM1-Premium-P1P2";
            if (cacheName.Contains("p3") || cacheName.Contains("p4")) return "MemtierBenchmarkM1-Premium-P1P2";
            if (cacheName.Contains("p5")) return "MemtierBenchmarkM1-Premium-P1P2";

            if (cacheName.Contains("c0") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c1") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c2") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c3") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c4") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c5") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c6") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";

            if (cacheName.Contains("c0") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c1") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c2") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c3") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c4") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c5") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c6") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";


            //if (cacheName.Contains("p1") || cacheName.Contains("p2")) return "MemtierBenchmarkM1-Premium-P1P2";
            //if (cacheName.Contains("p3") || cacheName.Contains("p4")) return "MemtierBenchmarkM2-Premium-P3P4";
            //if (cacheName.Contains("p5")) return "MemtierBenchmarkM3-Premium-P5";

            //if (cacheName.Contains("c0") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            //if (cacheName.Contains("c1") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            //if (cacheName.Contains("c2") && cacheName.Contains("standard")) return "MemtierBenchmarkM2-Standard-C2C3";
            //if (cacheName.Contains("c3") && cacheName.Contains("standard")) return "MemtierBenchmarkM2-Standard-C2C3";
            //if (cacheName.Contains("c4") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";
            //if (cacheName.Contains("c5") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";
            //if (cacheName.Contains("c6") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";

            //if (cacheName.Contains("c0") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            //if (cacheName.Contains("c1") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            //if (cacheName.Contains("c2") && cacheName.Contains("basic")) return "MemtierBenchmarkM2-Basic-C3C4";
            //if (cacheName.Contains("c3") && cacheName.Contains("basic")) return "MemtierBenchmarkM2-Basic-C3C4";
            //if (cacheName.Contains("c4") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";
            //if (cacheName.Contains("c5") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";
            //if (cacheName.Contains("c6") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";

            throw new ArgumentException($"Invalid cache name: {cacheName}");
        }
    }

}

