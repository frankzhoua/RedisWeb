using System.ComponentModel.DataAnnotations;

namespace redis.WebAPi.Model.BenchmarkModel
{
    public class BenchmarkFinalDataModel
    {
        public BenchmarkFinalDataModel() { }
        public string CacheName { get; set; }
        public double TotalDuration { get; set; }
        public string TimeUnit { get; set; }
        public double GetsRPS { get; set; }
        public double GetsAverageLatency { get; set; }
        public double GetsP50 { get; set; }
        public double GetsP99 { get; set; }
        public double GetsP99_90 { get; set; }
        public double GetsP99_99 { get; set; }
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
                ID = ID
            };
        }
    }

    
}
