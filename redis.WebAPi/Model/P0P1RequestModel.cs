namespace redis.WebAPi.Model
{
    public class P0P1RequestModel
    {
        public string region { get; set; }
        public string subscription { get; set; }
        public string group { get; set; }
        // 添加构造函数，从 RedisRequestModel 转换
        public P0P1RequestModel(RedisRequestModel redisRequest)
        {
            region = redisRequest.region ?? string.Empty;
            subscription = redisRequest.subscription ?? string.Empty;
            group = redisRequest.group ?? string.Empty;
        }
    }
}
