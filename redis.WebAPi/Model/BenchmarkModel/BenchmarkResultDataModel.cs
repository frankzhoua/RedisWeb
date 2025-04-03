using Newtonsoft.Json;

namespace redis.WebAPi.Model
{
     public class BenchmarkResultData
        {
        public string CacheName { get; set; }

        [JsonProperty("Total duration")]
        public double TotalDuration { get; set; }

        [JsonProperty("Time unit")]
        public string TimeUnit { get; set; }

        [JsonProperty("Gets RPS")]
        public double GetsRPS { get; set; }

        [JsonProperty("Gets average latency")]
        public double GetsAverageLatency { get; set; }

        [JsonProperty("Gets p50.00")]
        public double GetsP50 { get; set; }

        [JsonProperty("Gets p99.00")]
        public double GetsP99 { get; set; }

        [JsonProperty("Gets p99.90")]
        public double GetsP99_90 { get; set; }

        [JsonProperty("Gets p99.99")]
        public double GetsP99_99 { get; set; }

        [JsonProperty("Compressed Histogram")]
        public string CompressedHistogram { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ID { get; set; }
    }
   

}