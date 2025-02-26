using AutoMapper;
using redis.WebAPi.Model;
using redis.WebAPi.Model.BenchmarkModel;
using redis.WebAPi.Repository.AppDbContext;
using System.Text.RegularExpressions;

namespace redis.WebAPi.Service.Benchmark
{
    // Rename class to match constructor and class name
    public class InsertBenchmarkService 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InsertBenchmarkService> _logger;

        public InsertBenchmarkService(IServiceProvider serviceProvider, ILogger<InsertBenchmarkService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;   
        }

        // Insert the run result into the database
        public async Task InsertBenchmarkResultData(string output, string cacheName, DateTime timeStamp)
        {
            try
            {
                var entries = ExtractEntries(output);
                var resultDataList = new List<BenchmarkResultData>();
                var finalDataList = new List<BenchmarkFinalDataModel>();

                // 使用CreateScope()手动管理DbContext
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();

                    foreach (var entry in entries)
                    {
                        var benchmarkData = new BenchmarkResultData
                        {
                            CacheName = cacheName,
                            TotalDuration = entry.TotalDuration,
                            TimeUnit = entry.TimeUnit,
                            GetsRPS = entry.GetsRPS,
                            GetsAverageLatency = entry.GetsAverageLatency,
                            GetsP50 = entry.GetsP50,
                            GetsP99 = entry.GetsP99,
                            GetsP99_90 = entry.GetsP99_90,
                            GetsP99_99 = entry.GetsP99_99,
                            TimeStamp = timeStamp
                        };

                        resultDataList.Add(benchmarkData);

                        var finalData = new BenchmarkFinalDataModel(benchmarkData);
                        finalData.CacheName = cacheName + "Final";
                        finalDataList.Add(finalData);
                    }

                    // **批量插入**
                    if (resultDataList.Any())
                    {
                        await dbContext.BenchmarkResultData.AddRangeAsync(resultDataList);
                        //await dbContext.BenchmarkFinalData.AddRangeAsync(finalDataList);
                    }

                    // **统一提交，提高效率**
                    await dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Benchmark data inserted successfully.");
            }            
            catch (Exception ex)
            {
                _logger.LogInformation($"Error inserting benchmark data: {ex.Message}");
            }
        }

        private List<BenchmarkEntry> ExtractEntries(string output)
        {
            var entries = new List<BenchmarkEntry>();

            // Regular expressions are used to extract data related to each Entry
            var entryPattern = new Regex(@"Entry (\d+):.*?Total duration: (\d+)\s*.*?Time unit: (\w+).*?Gets RPS: ([\d\.]+).*?Gets average latency: ([\d\.]+).*?Gets p50\.00: ([\d\.]+).*?Gets p99\.00: ([\d\.]+).*?Gets p99\.90: ([\d\.]+).*?Gets p99\.99: ([\d\.]+)", RegexOptions.Singleline);

            var matches = entryPattern.Matches(output);

            foreach (Match match in matches)
            {
                var entry = new BenchmarkEntry
                {
                    TotalDuration = double.Parse(match.Groups[2].Value),
                    TimeUnit = match.Groups[3].Value,
                    GetsRPS = double.Parse(match.Groups[4].Value),
                    GetsAverageLatency = double.Parse(match.Groups[5].Value),
                    GetsP50 = double.Parse(match.Groups[6].Value),
                    GetsP99 = double.Parse(match.Groups[7].Value),
                    GetsP99_90 = double.Parse(match.Groups[8].Value),
                    GetsP99_99 = double.Parse(match.Groups[9].Value)
                };
                entries.Add(entry);
            }

            return entries;
        }
    }

    // Extract BenchmarkEntry outside the class as a normal class
    public class BenchmarkEntry
    {
        public double TotalDuration { get; set; }
        public string TimeUnit { get; set; }
        public double GetsRPS { get; set; }
        public double GetsAverageLatency { get; set; }
        public double GetsP50 { get; set; }
        public double GetsP99 { get; set; }
        public double GetsP99_90 { get; set; }
        public double GetsP99_99 { get; set; }
    }
}
