using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace redis.WebAPi.Model.BenchmarkModel
{
    public class BenchmarkFinalDataModel
    {
        public BenchmarkFinalDataModel() { }
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

        public BenchmarkFinalDataModel(BenchmarkResultData data)
        {
            CacheName = data.CacheName;
            TotalDuration = data.TotalDuration;
            TimeUnit = data.TimeUnit;
            GetsRPS = data.GetsRPS;
            GetsAverageLatency = data.GetsAverageLatency;
            GetsP50 = data.GetsP50;
            GetsP99 = data.GetsP99;
            GetsP99_90 = data.GetsP99_90;
            GetsP99_99 = data.GetsP99_99;
            CompressedHistogram = data.CompressedHistogram;
            TimeStamp = data.TimeStamp;
            ID = data.ID;
        }

        public BenchmarkResultData ToBenchmarkResultData()
        {
            return new BenchmarkResultData
            {
                CacheName = CacheName,
                TotalDuration = TotalDuration,
                TimeUnit = TimeUnit,
                GetsRPS = GetsRPS,
                GetsAverageLatency = GetsAverageLatency,
                GetsP50 = GetsP50,
                GetsP99 = GetsP99,
                GetsP99_90 = GetsP99_90,
                GetsP99_99 = GetsP99_99,
                TimeStamp = TimeStamp,
                CompressedHistogram = CompressedHistogram,
                ID = ID
            };
        }
    }

    
}
