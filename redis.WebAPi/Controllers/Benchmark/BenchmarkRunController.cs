using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Repository.AppDbContext; 
using redis.WebAPi.Model;
using redis.WebAPi.Service.IService;
using redis.WebAPi.Service;
using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Service.Benchmark;

namespace Benchmark_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenchmarkRunController : ControllerBase
    {
        private readonly BenchmarkContent _dbContext;  
        private readonly OperationSQL _benchmarkService;

        // Inject BenchmarkDbContext and ConnectionVMService through the constructor
        public BenchmarkRunController(BenchmarkContent dbContext, OperationSQL benchmarkService)
        {
            _dbContext = dbContext;
            _benchmarkService = benchmarkService;
        }

        // Receive the front-end parameters, then put them into the database and invoke the VM operation
        [HttpPost("enqueue")]
        public async Task<IActionResult> InvokeVMOperation([FromBody] BenchmarkRequestModel benchmarkRequest)
        {
            try
            {
                var benchmarkTask = new BenchmarkRequestModel
                {
                    Name = benchmarkRequest.Name,
                    Clients = benchmarkRequest.Clients,
                    Threads = benchmarkRequest.Threads,
                    Size = benchmarkRequest.Size,
                    Requests = benchmarkRequest.Requests,
                    Pipeline = benchmarkRequest.Pipeline,
                    Times = benchmarkRequest.Times,
                    TimeStamp = DateTime.Now,
                    Status = 2  
                };

                _dbContext.BenchmarkQueue.Add(benchmarkTask);
                _dbContext.BenchmarkRequest.Add(benchmarkTask);
                await _dbContext.SaveChangesAsync();

                return Ok("Task has been enqueued.");
            }
            catch (Exception ex)
            {
                var parameters = await _dbContext.BenchmarkRequest.FirstOrDefaultAsync(p => p.Name == benchmarkRequest.Name && p.TimeStamp == benchmarkRequest.TimeStamp);

                if (parameters != null)
                {
                    parameters.Status = 4;  // Status 4 indicates failure
                    await _dbContext.SaveChangesAsync();
                }
                await _benchmarkService.UpdateCacheStatus (benchmarkRequest.Name, SQLDataBaseEnum.BenchmarkRequest,  4);
                return StatusCode(500, new { message = "Error occurred during benchmark execution", error = ex.Message });
            }
        }
    }
}
