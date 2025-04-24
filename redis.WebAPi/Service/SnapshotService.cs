using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Model.TestCase;
using redis.WebAPi.Repository.AppDbContext;

namespace redis.WebAPi.Service
{
    public class SnapshotService
    {

        private readonly IServiceProvider _serviceProvider;

        public SnapshotService(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SaveSnapshotAsync(string planName, List<TestCaseEntity> testCases, string operatorName, string comment)
        {
            var snapshot = new SnapshotEntity
            {
                PlanName = planName,
                Operator = operatorName,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                TestCases = testCases.Select(tc => new TestCaseEntity
                {
                    TestCaseId = tc.TestCaseId,
                    Title = tc.Title,
                    AssignedTo = tc.AssignedTo,
                    SuiteId = tc.SuiteId,
                    SuiteName = tc.SuiteName,
                    Steps = tc.Steps
                }).ToList()
            };
            using (var scope = _serviceProvider.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                dbContext.Snapshot.Add(snapshot);
                dbContext.SaveChanges();

            }
        }

        public async Task<List<TestCaseEntity>> GetLatestSnapshotAsync(string planName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BenchmarkContent>();
                var snapshot = await dbContext.Snapshot
       .Include(s => s.TestCases)
       .Where(s => s.PlanName == planName)
       .OrderByDescending(s => s.CreatedAt)
       .FirstOrDefaultAsync();

                return snapshot?.TestCases.Select(tc => new TestCaseEntity
                {
                    TestCaseId = tc.TestCaseId,
                    Title = tc.Title,
                    AssignedTo = tc.AssignedTo,
                    SuiteId = tc.SuiteId,
                    SuiteName = tc.SuiteName,
                    Steps = tc.Steps
                }).ToList();

            }     
        }

        public List<TestCaseEntity> Diff(List<TestCaseEntity> oldList, List<TestCaseEntity> newList)
        {
            var diff = newList.Where(n =>
                !oldList.Any(o => o.TestCaseId == n.TestCaseId &&
                                  o.Title == n.Title &&
                                  o.AssignedTo == n.AssignedTo &&
                                  o.Steps == n.Steps)).ToList();
            return diff;
        }

    }
}
