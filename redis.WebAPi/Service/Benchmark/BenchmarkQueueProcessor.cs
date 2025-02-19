using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Repository.AppDbContext;
using redis.WebAPi.Service.AzureShared;

namespace redis.WebAPi.Service.Benchmark
{
    public class BenchmarkQueueProcessor : IHostedService, IDisposable
    {
        private readonly BenchmarkContent _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public BenchmarkQueueProcessor(BenchmarkContent dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 中：启动定时任务，每隔一定时间检查任务队列
            _timer = new Timer(ExecutePendingTasks, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));  // 每30秒检查一次队列
            return Task.CompletedTask;
        }

        private async void ExecutePendingTasks(object state)
        {
            try
            {
                // 中：获取所有待处理的任务
                var benchmarkTask = await _dbContext.BenchmarkQueue
                    .Where(b => b.Status == 2) // 中：只选择待处理的任务
                    .OrderBy(b => b.TimeStamp)
                    .FirstOrDefaultAsync();

                if (benchmarkTask != null)
                {
                    // 中：找到任务并执行
                    var connectionVMService = _serviceProvider.GetRequiredService<ConnectionVMService>();
                    string output = await connectionVMService.ConnectionVM();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing benchmark tasks: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
