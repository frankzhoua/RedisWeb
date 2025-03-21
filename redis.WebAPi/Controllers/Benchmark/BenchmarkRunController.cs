using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Repository.AppDbContext; 
using redis.WebAPi.Model;
using redis.WebAPi.Service.IService;
using redis.WebAPi.Service;
using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Service.Benchmark;
using redis.WebAPi.Model.BenchmarkModel;
using redis.WebAPi.Service.AzureShared;
using System.Threading;
using Polly;

namespace Benchmark_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenchmarkRunController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionVMService _connectionVMService;

        // Inject BenchmarkDbContext and ConnectionVMService through the constructor
        public BenchmarkRunController(ConnectionVMService connectionVMService, IServiceProvider serviceProvider  )
        {
            _connectionVMService = connectionVMService;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("execute-tasks")]
        public async Task<IActionResult> ExecutePendingTasks()
        {
            try
            {
                await _connectionVMService.ExecuteTasksOnVMs();
                string result = "OK";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error executing tasks: {ex.Message}");
            }
        }

        [HttpPost("FlushQueue")]
        public async Task<IActionResult> FlushTask()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope()) 
                {
                    var dbContext = scope.ServiceProvider.GetService<BenchmarkContent>();
                    dbContext.BenchmarkQueue.RemoveRange();
                    dbContext.SaveChanges();
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error executing tasks: {ex.Message}");
            }
        }

        [HttpPost("FinalDataTest")]
        public async Task<IActionResult> FinalData()
        {
            try
            {
                _connectionVMService.FinalDataCollection(DateTime.Now);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error executing tasks: {ex.Message}");
            }
        }

        [HttpPost("disposiable run")]
        public async Task<IActionResult> runOneQueue()
        {
            try
            {
                var vm = await _connectionVMService.GetVirtualMachineAsync("MemtierBenchmarkTest");
                var request = new BenchmarkQueueDataModel() 
                {
                    Name= "benchmarktest",
                    pw = "4rqcibERjQdcEtaqbk7ytwue1nhjApY54AzCaI4S0Y0=",
                    Clients = 4 ,
                    Threads = 16,
                    Size = 1024,
                    Requests = 10000 ,
                    Pipeline = 20 ,
                    Times = 5                
                };
                var result = await _connectionVMService.RunBenchmarkOnVM(vm,request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error executing tasks: {ex.Message}");
            }
        }

        // Receive the front-end parameters, then put them into the database and invoke the VM operation
        [HttpPost("enqueue")]
        public async Task<IActionResult> InvokeVMOperation([FromBody] BenchmarkRequestModel benchmarkRequest)
        {
            try
            {
                for (int i=1; i<=benchmarkRequest.Times;i++) 
                {
                    var benchmarkTask = new BenchmarkRequestModel
                    {
                        Name = benchmarkRequest.Name + i,
                        Region = benchmarkRequest.Region,
                        Clients = benchmarkRequest.Clients,
                        Threads = benchmarkRequest.Threads,
                        Size = benchmarkRequest.Size,
                        Requests = benchmarkRequest.Requests,
                        Pipeline = benchmarkRequest.Pipeline,
                        Times = benchmarkRequest.Times,
                        TimeStamp = DateTime.Now,
                        Status = 2
                    };
                    var benchmarkQueue = new BenchmarkQueueDataModel(benchmarkTask);

                    _dbContext.BenchmarkQueue.Add(benchmarkQueue);
                    _dbContext.BenchmarkRequest.Add(benchmarkTask);

                }
                await _dbContext.SaveChangesAsync();

                return Ok("Task has been enqueued.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error occurred during benchmark execution", error = ex.Message });
            }
        }




    }
}
