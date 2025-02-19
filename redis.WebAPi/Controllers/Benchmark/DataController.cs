using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Repository.AppDbContext;

namespace Benchmark_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly BenchmarkContent _dbContext;

        public DataController(BenchmarkContent dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("AllData")]
        public async Task<IActionResult> GetAllData([FromQuery] DateTime TimeStamp)
        {
            
            var query = _dbContext.BenchmarkResultData.AsQueryable();

            if (!string.IsNullOrEmpty(TimeStamp.ToString()))
            {
                query = query.Where(b => b.TimeStamp == TimeStamp);
            }

            var data = await query
                .OrderBy(p => p.ID)
                .Select(b => new
                {
                    b.CacheName,
                    b.TotalDuration,
                    b.TimeUnit,
                    b.GetsRPS,
                    b.GetsAverageLatency,
                    b.GetsP50,
                    b.GetsP99,
                    b.GetsP99_90,
                    b.GetsP99_99,
                    b.ID
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("GetBenchmarkRequestData")]
        public async Task<IActionResult> GetBenchmarkRequestData()
        {
            // Use Entity Framework to query the Parameters table
            var parameters = await _dbContext.BenchmarkRequest
                .Select(p => new
                {
                    p.Name,
                    p.Region,
                    Description = p.Description ?? "No Description",
                    p.Clients,
                    p.Threads,
                    p.Size,
                    p.Requests,
                    p.Pipeline,
                    p.Status,
                    p.TimeStamp
                })
                .ToListAsync();

            return Ok(parameters);
        }

    }
}
