using Azure.Core;
using Azure.ResourceManager.Compute.Models;
using Azure;
using Azure.ResourceManager.Compute;
using redis.WebAPi.Service.Benchmark;
using redis.WebAPi.Repository.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Threading;
using redis.WebAPi.Model;
using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Model.BenchmarkModel;
using Polly;
using Microsoft.EntityFrameworkCore.Migrations.Operations;



namespace redis.WebAPi.Service.AzureShared
{
    public class ConnectionVMService 
    {
        private readonly AzureClientFactory _client;
        private readonly InsertBenchmarkService _insertBenchmarkService;
        //private readonly BenchmarkContent _dbContext;
        private readonly ILogger<ConnectionVMService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly OperationSQL _sqlOperation;


        public ConnectionVMService(AzureClientFactory client, InsertBenchmarkService insertBenchmarkService, ILogger<ConnectionVMService> logger, IServiceProvider serviceProvider, OperationSQL sqlOperation, BenchmarkContent dbContext)
        {
            //_dbContext = dbContext;
            _client = client;
            _insertBenchmarkService = insertBenchmarkService;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sqlOperation = sqlOperation;
        }

        private async Task<Dictionary<String,List<BenchmarkQueueDataModel>>> DistributeTasksIntoLists(
            Dictionary<string, List<BenchmarkQueueDataModel>> vmTaskLists)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _logger.LogInformation("\n 执行分配！！！！！！！");
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();

                var tasks = await dbContext.BenchmarkQueue
                    .Where(t => t.Status == 2)  
                    .OrderBy(t => t.Id)  
                    .ToListAsync();
                _logger.LogInformation("\n 执行分配完毕！！！！");
                foreach (var task in tasks)
                {

                    var vmResource = await GetVMByCacheName(task.Name);
                    if (vmResource == null)
                    {
                        
                        continue;
                    }

                    string vmName = vmResource.Data.Name; 

                    // create a new key if there is no vmName kay
                    if (!vmTaskLists.ContainsKey(vmName))
                    {
                        vmTaskLists[vmName] = new List<BenchmarkQueueDataModel>();
                    }

                    vmTaskLists[vmName].Add(task);
                }
                return vmTaskLists;
            }
        }

        public async Task  ExecuteTasksOnVMs()
        {
            Dictionary<string, List<BenchmarkQueueDataModel>> vmTaskLists = new Dictionary<string, List<BenchmarkQueueDataModel>>();
            var dict = await DistributeTasksIntoLists(vmTaskLists);

            _logger.LogInformation("当前DIC值: " + string.Join(", ", dict.Select(kv => $"{kv.Key}: [{string.Join(", ", kv.Value)}]")));

            // List<Task> storage VM tasks
            List<Task> vmTasks = new List<Task>();

            foreach (var kvp in dict)
            {
                string vmName = kvp.Key;
                List<BenchmarkQueueDataModel> taskList = kvp.Value;

                Task vmTask = Task.Run(async () =>
                {
                    List<string> results = new List<string>();

                    foreach (var task in taskList)
                    {
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                                _logger.LogInformation($"当前任务：{task.Name}, 当前进程：{Thread.CurrentThread.ManagedThreadId}");

                                task.Status = 1;
                                dbContext.BenchmarkQueue.Update(task);
                                await dbContext.SaveChangesAsync();

                                string output = await RunTasksForVM(vmName, task);
                                results.Add($"[{vmName}] {output}");
                                _logger.LogInformation($"任务：{task.Name} 结束");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"执行 {task.Name} 时发生错误: {ex.Message}");
                        }
                    }
                });

                vmTasks.Add(vmTask); 
            }

            await Task.WhenAll(vmTasks); 
        }

        private async Task<string> RunTasksForVM(string vmName, BenchmarkQueueDataModel task)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope()) 
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                    string output = await ConnectionVMTest(task, dbContext);
                    return output;
                }
                    
            }
            catch (Exception ex) 
            {
                _logger.LogError($"执行任务失败: {task.Name}, 错误: {ex.Message}");

                task.Status = 4; 
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                    dbContext.BenchmarkRequest.Update(task.ToBenchmarkRequestModel());
                    await dbContext.SaveChangesAsync();
                }
                return ex.Message; 
            }

        }
     

     

        public async Task<string> ConnectionVMTest(BenchmarkQueueDataModel request,BenchmarkContent dbContext) 
        {


            if (request == null)
            {
                return "No pending benchmark tasks found.";
            }

            string cacheName = request.Name;

            _logger.LogInformation("\n 运行到SamulateBenchmarkByCacheName了！！！！！！！！！！！");
            var output = await SamulateBenchmarkByCacheName(cacheName,dbContext);
            return output;



        }

        public async Task<String> SamulateBenchmarkByCacheName(string cacheName,BenchmarkContent _dbContext)
        {

            await Task.Delay(TimeSpan.FromSeconds(10));
            var timeStamp = DateTime.Now;
            string fileName = $"output-{timeStamp}";

            var output = $"Entry 1:\r\nTotal duration: 222440\r\nTime unit: MILLISECONDS\r\nGets RPS: 151074.41\r\nGets average latency: 4.539\r\nGets p50.00: 3.823\r\nGets p99.00: 10.431\r\nGets p99.90: 14.719\r\nGets p99.99: 25.855\r\nEntry 2:\r\nTotal duration: 379985\r\nTime unit: MILLISECONDS\r\nGets RPS: 153115.65\r\nGets average latency: 7.531\r\nGets p50.00: 7.071\r\nGets p99.00: 14.015\r\nGets p99.90: 18.175\r\nGets p99.99: 28.159\r\nEntry 3:\r\nTotal duration: 365442\r\nTime unit: MILLISECONDS\r\nGets RPS: 159209.27\r\nGets average latency: 7.345\r\nGets p50.00: 7.071\r\nGets p99.00: 11.007\r\nGets p99.90: 14.655\r\nGets p99.99: 21.119\r\nEntry 4:\r\nTotal duration: 364630\r\nTime unit: MILLISECONDS\r\nGets RPS: 159563.51\r\nGets average latency: 7.313\r\nGets p50.00: 7.071\r\nGets p99.00: 9.855\r\nGets p99.90: 13.055\r\nGets p99.99: 17.663\r\nEntry 5:\r\nTotal duration: 360140\r\nTime unit: MILLISECONDS\r\nGets RPS: 161553.18\r\nGets average latency: 7.095\r\nGets p50.00: 7.071\r\nGets p99.00: 10.879\r\nGets p99.90: 14.015\r\nGets p99.99: 19.327\r\nEntry 6:\r\nTotal duration: 356755\r\nTime unit: MILLISECONDS\r\nGets RPS: 163085.93\r\nGets average latency: 6.928\r\nGets p50.00: 6.975\r\nGets p99.00: 11.199\r\nGets p99.90: 13.951\r\nGets p99.99: 18.303\r\nEntry 7:\r\nTotal duration: 350010\r\nTime unit: MILLISECONDS\r\nGets RPS: 166228.88\r\nGets average latency: 7.169\r\nGets p50.00: 7.071\r\nGets p99.00: 10.303\r\nGets p99.90: 13.951\r\nGets p99.99: 19.583\r\nEntry 8:\r\nTotal duration: 339696\r\nTime unit: MILLISECONDS\r\nGets RPS: 171275.73\r\nGets average latency: 6.863\r\nGets p50.00: 7.007\r\nGets p99.00: 11.135\r\nGets p99.90: 14.399\r\nGets p99.99: 19.711\r\nEntry 9:\r\nTotal duration: 334390\r\nTime unit: MILLISECONDS\r\nGets RPS: 173993.89\r\nGets average latency: 6.847\r\nGets p50.00: 5.695\r\nGets p99.00: 11.391\r\nGets p99.90: 14.847\r\nGets p99.99: 19.583\r\nEntry 10:\r\nTotal duration: 316090\r\nTime unit: MILLISECONDS\r\nGets RPS: 184067.21\r\nGets average latency: 6.82\r\nGets p50.00: 6.047\r\nGets p99.00: 11.135\r\nGets p99.90: 13.951\r\nGets p99.99: 20.223";

            if (!string.IsNullOrEmpty(output))
            {
                string newcacheName = cacheName+ Thread.CurrentThread.ToString();
                _logger.LogInformation("\n 运行到InsertBenchmarkResultData了！！！！！！！！！！！,线程： "+Thread.CurrentThread.ManagedThreadId.ToString());
                await _insertBenchmarkService.InsertBenchmarkResultData(output, newcacheName, timeStamp);
                var removedData = await _dbContext.BenchmarkQueue.FirstOrDefaultAsync(u => u.Name == cacheName);
                if (removedData != null)
                {
                    _logger.LogInformation("\n 运行到Remove了！！！！！！！！！！！线程： "+Thread.CurrentThread.ManagedThreadId.ToString());
                    _dbContext.BenchmarkQueue.Remove(removedData);
                    await _dbContext.SaveChangesAsync();
                }

            }

            return output;
        }



        //public async Task<string> ConnectionVM()
        //{
        //    try
        //    {
        //        var benchmarkTask = await _dbContext.BenchmarkQueue
        //            .Where(b => b.Status == 2)  // 只处理待处理任务
        //            .OrderBy(b => b.TimeStamp)  // 按照时间戳排序，先处理早期任务
        //            .FirstOrDefaultAsync();

        //        if (benchmarkTask == null)
        //        {
        //            return "No pending benchmark tasks found.";
        //        }

        //        string cacheName = benchmarkTask.Name;
        //        string primary = benchmarkTask.pw;
        //        int clients = benchmarkTask.Clients;
        //        int threads = benchmarkTask.Threads;
        //        int size = benchmarkTask.Size;
        //        int requests = benchmarkTask.Requests;
        //        int pipeline = benchmarkTask.Pipeline;
        //        int times = benchmarkTask.Times;

        //        var vm = await GetVMByCacheName(cacheName);
        //        if (!await IsVMAvailableForTask(vm))
        //        {
        //            throw new InvalidOperationException("VM is busy with another task.");
        //        }

        //        // 运行基准测试
        //        string output = await RunBenchmarkOnVM(vm, cacheName, primary, clients, threads, size, requests, pipeline, times);

        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        throw;
        //    }
        //}

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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                    var removedData = await dbContext.BenchmarkQueue.FirstOrDefaultAsync(u => u.Name == name);
                    if (removedData != null)
                    {
                        dbContext.BenchmarkQueue.Remove(removedData);
                        await dbContext.SaveChangesAsync();
                    }
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
            _logger.LogInformation("\n 执行获取虚拟机！！！！");
            string vmName = AllocateVMByCacheName(cacheName);
            _logger.LogInformation("\n 虚拟机获取完毕，当前虚拟机："+vmName);
            return await GetVirtualMachineAsync(vmName);
        }

        private string AllocateVMByCacheName(string cacheName)
        {
            cacheName = cacheName.ToLower();

            if (cacheName.Contains("p1") || cacheName.Contains("p2")) return "MemtierBenchmarkM1-Premium-P1P2";
            if (cacheName.Contains("p3") || cacheName.Contains("p4")) return "MemtierBenchmarkM2-Premium-P3P4";
            if (cacheName.Contains("p5")) return "MemtierBenchmarkM3-Premium-P5";

            if (cacheName.Contains("c0") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c1") && cacheName.Contains("standard")) return "MemtierBenchmarkM1-Standard-C0C1";
            if (cacheName.Contains("c2") && cacheName.Contains("standard")) return "MemtierBenchmarkM2-Standard-C2C3";
            if (cacheName.Contains("c3") && cacheName.Contains("standard")) return "MemtierBenchmarkM2-Standard-C2C3";
            if (cacheName.Contains("c4") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";
            if (cacheName.Contains("c5") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";
            if (cacheName.Contains("c6") && cacheName.Contains("standard")) return "MemtierBenchmarkM3-Standard-C4C5C6";

            if (cacheName.Contains("c0") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c1") && cacheName.Contains("basic")) return "MemtierBenchmarkM1-Basic-C0C1";
            if (cacheName.Contains("c2") && cacheName.Contains("basic")) return "MemtierBenchmarkM2-Basic-C3C4";
            if (cacheName.Contains("c3") && cacheName.Contains("basic")) return "MemtierBenchmarkM2-Basic-C3C4";
            if (cacheName.Contains("c4") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";
            if (cacheName.Contains("c5") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";
            if (cacheName.Contains("c6") && cacheName.Contains("basic")) return "MemtierBenchmarkM3-Basic-C4C5C6";

            throw new ArgumentException($"Invalid cache name: {cacheName}");
        }
    }

}

