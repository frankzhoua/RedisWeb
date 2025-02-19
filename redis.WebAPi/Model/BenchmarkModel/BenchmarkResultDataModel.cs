namespace redis.WebAPi.Model
{
     public class BenchmarkResultData
        {
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
    }
   

}