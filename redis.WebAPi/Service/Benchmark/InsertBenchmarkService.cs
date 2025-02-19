using redis.WebAPi.Model;
using redis.WebAPi.Repository.AppDbContext;
using System.Text.RegularExpressions;

namespace redis.WebAPi.Service.Benchmark
{
    // Rename class to match constructor and class name
    public class InsertBenchmarkService 
    {
        private readonly BenchmarkContent _context;

        public InsertBenchmarkService(BenchmarkContent context)
        {
            _context = context;
        }

        // Insert the run result into the database
        public async Task InsertBenchmarkResultData(string output, string cacheName, DateTime timeStamp)
        {
            try
            {
                // Extract all Entry blocks
                var entries = ExtractEntries(output);

                // Iterate through each Entry, generate a BenchmarkRequestData object and insert it into the database
                foreach (var entry in entries)
                {
                    var BenchmarkData1 = new BenchmarkResultData
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
                    await _context.BenchmarkResultData.AddAsync(BenchmarkData1);
                }
                await _context.SaveChangesAsync();
                Console.WriteLine("Benchmark data inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting benchmark data: {ex.Message}");
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
