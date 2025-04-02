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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static OfficeOpenXml.ExcelErrorValue;
using redis.WebAPi.Models;
using System.Text.RegularExpressions;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Redis;



namespace redis.WebAPi.Service.AzureShared
{
    public class ConnectionVMService 
    {
        private readonly AzureClientFactory _client;
        //private readonly BenchmarkContent _dbContext;
        private readonly ILogger<ConnectionVMService> _logger;
        private readonly IServiceProvider _serviceProvider;


        public ConnectionVMService(AzureClientFactory client, ILogger<ConnectionVMService> logger, IServiceProvider serviceProvider, BenchmarkContent dbContext)
        {
            //_dbContext = dbContext;
            _client = client;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        private async Task<Dictionary<String,List<BenchmarkQueueDataModel>>> DistributeTasksIntoLists(
            Dictionary<string, List<BenchmarkQueueDataModel>> vmTaskLists)
        {
            /*
                This method obtains the task in the BenchmarkQueue table with state 2 (pending) from the database and assigns it to a different virtual machine (VM) based on the task name.
                AllocateVMByCacheName Used to assign a VM name based on the task name.
                Add each task to the vmTaskLists dictionary according to the assigned virtual machine name.
             */
            using (var scope = _serviceProvider.CreateScope())
            {
                _logger.LogInformation("\n ִ�з��䣡������������");
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();

                var tasks = await dbContext.BenchmarkQueue
                    .Where(t => t.Status == 2)  
                    .OrderBy(t => t.Id)  
                    .ToListAsync();
                _logger.LogInformation("\n ִ�з�����ϣ�������");
                foreach (var task in tasks)
                {

                    string vmName = AllocateVMByCacheName(task.Name); 

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

        public async Task<List<BenchmarkQueueDataModel>> GeneratingQCommendByGroupResourse(string group) 
        {
            var groupResourse = _client.GetGroup(group);
            var redisCollection = groupResourse.GetAllRedis();
            List<BenchmarkQueueDataModel> listQ = new List<BenchmarkQueueDataModel>();
            foreach (var redis in redisCollection) 
            {
                var queue = new BenchmarkQueueDataModel
                {
                    Name = redis.Data.Name,
                    Clients = 64,
                    Threads = 16,
                    Requests = 100000,
                    Size = 1024,
                    Pipeline = redis.Data.Name.Contains("Premium") ? 20:10,
                    pw = redis.GetKeys().Value.PrimaryKey,
                    Status =1,
                    TimeStamp = DateTime.Now,
                    Times = 10,
                    Region = "East US 2 EUAP"
                  
                };
                listQ.Add(queue);
            }
            return listQ;
        }

        public async Task  ExecuteTasksOnVMs()
        {
            /*
                This method gets the list of assigned tasks from DistributeTasksIntoLists.
                Create an asynchronous task for each virtual machine task and perform the corresponding benchmark.
                In each task, update the task status to 1 (executing) and call RunTasksForVM to perform the specific action.
                Exception handling: If the task fails to be executed, set the task status to 4 (failed) and record an error message.
             */
            Dictionary<string, List<BenchmarkQueueDataModel>> vmTaskLists = new Dictionary<string, List<BenchmarkQueueDataModel>>();
            var dict = await DistributeTasksIntoLists(vmTaskLists);

            _logger.LogInformation("Dic: " + string.Join(", ", dict.Select(kv => $"{kv.Key}: [{string.Join(", ", kv.Value)}]")));

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
                                _logger.LogInformation($"��ǰ����{task.Name}, ��ǰ���̣�{Thread.CurrentThread.ManagedThreadId}");

                                task.Status = 1;
                                dbContext.BenchmarkQueue.Update(task);
                                await dbContext.SaveChangesAsync();

                                string output = await RunTasksForVM(task);
                                results.Add($"[{vmName}] {output}");

                                _logger.LogInformation($"����{task.Name} ����");
                            }
                        }
                        catch (Exception ex)
                        {
                            task.Status = 4;
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                                dbContext.BenchmarkQueue.Update(task);
                                await dbContext.SaveChangesAsync();
                            }
                            _logger.LogError($"ִ�� {task.Name} ʱ��������: {ex.Message}");
                            continue;
                        }
                    }
                });

                vmTasks.Add(vmTask); 
            }

            await Task.WhenAll(vmTasks); 
        }

        private async Task<string> RunTasksForVM(BenchmarkQueueDataModel task)
        {
            /*
                This method handles the execution of each task. First try running the ConnectionVMTest method to get the benchmark results.
                If an exception occurs, the task status is updated to 4 (failed) and an error message is returned.
             */
            try
            {
                using (var scope = _serviceProvider.CreateScope()) 
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                    string output = await ConnectionVMTest(task, dbContext);
                    task.Status = 3;
                    dbContext.BenchmarkRequest.Update(task.ToBenchmarkRequestModel());
                    await dbContext.SaveChangesAsync(); // �������
                    return output;
                }
                    
            }
            catch (Exception ex) 
            {
                _logger.LogError($"ִ������ʧ��: {task.Name}, ����: {ex.Message}");

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

        //Connect virtual machines and perform benchmarks (ConnectionVMTest and RunBenchmarkOnVM)
        public async Task<string> ConnectionVMTest(BenchmarkQueueDataModel request,BenchmarkContent dbContext) 
        {


            if (request == null)
            {
                return "No pending benchmark tasks found.";
            }

            string cacheName = request.Name;
            cacheName = Regex.Replace(cacheName, @"\d$", "");  
            var vm = GetVMByCacheName(cacheName).Result;
            var output = await RunBenchmarkOnVM(vm, request);
            return output;



        }


        public async Task FinalDataCollection(DateTime targetDate)
        {

            using (var scope = _serviceProvider.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                var allData = dbContext.BenchmarkResultData.AsNoTracking()
                    .Where(d => d.TimeStamp.Date == targetDate.Date)
                    .ToList();

                if (!allData.Any()) return; 

                // ������� SKU ����λ��
                var medianResults = allData
                    .Where(data => !string.IsNullOrEmpty(data.CacheName))
                    .GroupBy(data => ExtractSku(data.CacheName))
                    .Select(group => new BenchmarkFinalDataModel
                    {
                        CacheName = group.Key,
                        TotalDuration = CalculateMedian(group.Select(d => d.TotalDuration).ToList()),
                        TimeUnit = "MILLISECONDS",
                        GetsRPS = CalculateMedian(group.Select(d => d.GetsRPS).ToList()),
                        GetsAverageLatency = CalculateMedian(group.Select(d => d.GetsAverageLatency).ToList()),
                        GetsP50 = CalculateMedian(group.Select(d => d.GetsP50).ToList()),
                        GetsP99 = CalculateMedian(group.Select(d => d.GetsP99).ToList()),
                        GetsP99_90 = CalculateMedian(group.Select(d => d.GetsP99_90).ToList()),
                        GetsP99_99 = CalculateMedian(group.Select(d => d.GetsP99_99).ToList()),
                        TimeStamp = DateTime.Now
                    })
                    .ToList();
                dbContext.BenchmarkFinalData.AddRange(medianResults);
                dbContext.SaveChanges();
            }
        }

        private string ExtractSku(string cacheName)
        {
            var parts = cacheName.Split('-'); 
            return parts.Length > 1 ? parts[1] : "Unknown"; 
        }

        private double CalculateMedian(List<double> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            values.Sort(); 

            int count = values.Count;
            int mid = count / 2;

            if (count % 2 == 0) 
                return (values[mid - 1] + values[mid]) / 2.0;
            else 
                return values[mid];
        }


        public async Task<string> RunBenchmarkOnVM(VirtualMachineResource vm, BenchmarkQueueDataModel request)
        {
            var timeStamp = DateTime.Now;
            string fileName = $"output-{timeStamp}";
            string cacheName = request.Name;
            cacheName = Regex.Replace(cacheName, @"\d$", "");

            var runCommandInput = new RunCommandInput("RunShellScript")
            {
                Script =
            {
                $"memtier_benchmark -h {cacheName} -a {request.pw} --threads {request.Threads} --clients {request.Clients} -n {request.Requests} --ratio=1:10 --pipeline {request.Pipeline} -d {request.Size} --random-data --key-pattern=S:S --key-minimum=1 --key-maximum=10000 -x 1 --print-percentiles 50,99,99.9,99.99 --json-out-file /home/azureuser/out.json",
               
            }

            };

            await vm.RunCommandAsync(WaitUntil.Completed, runCommandInput);
            var runCommandInput2 = new RunCommandInput("RunShellScript")
            {
                Script = { "jq '{\r\n    \"Total duration\": .[\"ALL STATS\"].Runtime[\"Total duration\"],\r\n    \"Time unit\": .[\"ALL STATS\"].Runtime[\"Time unit\"],\r\n    \"Gets RPS\": .[\"ALL STATS\"].Gets[\"Ops/sec\"],\r\n    \"Gets average latency\": .[\"ALL STATS\"].Gets[\"Average Latency\"],\r\n    \"Gets p50.00\": .[\"ALL STATS\"].Gets[\"Percentile Latencies\"][\"p50.00\"],\r\n    \"Gets p99.00\": .[\"ALL STATS\"].Gets[\"Percentile Latencies\"][\"p99.00\"],\r\n    \"Gets p99.90\": .[\"ALL STATS\"].Gets[\"Percentile Latencies\"][\"p99.90\"],\r\n    \"Gets p99.99\": .[\"ALL STATS\"].Gets[\"Percentile Latencies\"][\"p99.99\"]\r\n}' /home/azureuser/out.json" }

            };
            var output = (await vm.RunCommandAsync(WaitUntil.Completed, runCommandInput2)).Value.Value.Select(r=>r.Message).First();
            
            //var output = string.Join("\n", response.Value.Select(r => r.Message));
            int startIndex = output.IndexOf("{");
            int endIndex = output.LastIndexOf("}");
            
            string jsonPart = output.Substring(startIndex-1, endIndex - startIndex + 2); 
            
            var savedData = JsonConvert.DeserializeObject<BenchmarkResultData>(jsonPart);
            savedData.CacheName = request.Name;
            savedData.TimeStamp = DateTime.Now;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                var removedData = await dbContext.BenchmarkQueue.FirstOrDefaultAsync(u => u.Name == request.Name);
                if (removedData != null )
                {
                    dbContext.BenchmarkQueue.Remove(removedData);
                    await dbContext.SaveChangesAsync();
                }
                dbContext.BenchmarkResultData.Add(savedData);
                await dbContext.SaveChangesAsync();
            }
            output = JsonConvert.SerializeObject(savedData);
            return output;
        }




        public async Task<VirtualMachineResource> GetVirtualMachineAsync(string vmName)
        {
            var armClient = _client.ArmClient;
            //var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "1e57c478-0901-4c02-8d35-49db234b78d2"));
            var subResource = armClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + "fc2f20f5-602a-4ebd-97e6-4fae3f1f6424"));
            var vmResource = (await subResource.GetResourceGroupAsync("MemtierbenchmarkTest")).Value.GetVirtualMachines().GetAsync(vmName).Result;
          

            return vmResource;
        }



        public async Task<VirtualMachineResource> GetVMByCacheName(string cacheName)
        {
            _logger.LogInformation("\n ִ�л�ȡ�������������");
            string vmName = AllocateVMByCacheName(cacheName);
            _logger.LogInformation("\n �������ȡ��ϣ���ǰ�������"+vmName);
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

