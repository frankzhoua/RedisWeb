using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using redis.WebAPi.Model;
using redis.WebAPi.Repository.AppDbContext;
using redis.WebAPi.Service.IService;
using redis.WebAPi.Model.TestCase;

namespace redis.WebAPi.Service
{
    public class TestPlanDiff : ITestPlanDiff
    {

        string collectionUri = "https://msazure.visualstudio.com/";
        private readonly string _project = "RedisCache";
        private readonly IServiceProvider _serviceProvider;

        public TestPlanDiff(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<TestCaseEntity>> GetAllTestCasesAsync(string pat, string planName)
        {
            var connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, pat));
            var planClient = connection.GetClient<TestPlanHttpClient>();
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var plans = await planClient.GetTestPlansAsync(_project);
            Console.WriteLine("跑完GetAllSuitesRecursive 了");
            var plan = plans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));
            if (plan == null) throw new Exception($"未找到计划: {planName}");

            var allSuites = new List<TestSuite>();
            await GetAllSuitesRecursive(planClient, plan.Id, plan.RootSuite.Id, allSuites);

            var allTestCases = new List<TestCaseEntity>();
            foreach (var suite in allSuites)
            {
                var refs = await planClient.GetTestCaseListAsync(_project, plan.Id, suite.Id);
                var ids = refs.Select(tc => tc.workItem.Id).ToList();
                if (ids.Count == 0) continue;

                var items = await witClient.GetWorkItemsAsync(ids);
                foreach (var item in items)
                {
                    allTestCases.Add(new TestCaseEntity
                    {
                        TestCaseId = item.Id.Value,
                        Title = item.Fields["System.Title"]?.ToString(),
                        AssignedTo = item.Fields.ContainsKey("System.AssignedTo") ? item.Fields["System.AssignedTo"]?.ToString() : string.Empty,
                        SuiteId = suite.Id,
                        SuiteName = suite.Name,
                        Steps = item.Fields.ContainsKey("Microsoft.VSTS.TCM.Steps") ? item.Fields["Microsoft.VSTS.TCM.Steps"]?.ToString() : string.Empty
                    });
                }
            }

            return allTestCases;

        }


        private async Task GetAllSuitesRecursive(TestPlanHttpClient client, int planId, int parentSuiteId, List<TestSuite> all)
        {
            var allSuites = await client.GetTestSuitesForPlanAsync(_project, planId);

            // 只处理当前parentSuiteId的直接子套件
            var childSuites = allSuites.Where(suite => suite.ParentSuite?.Id == parentSuiteId);

            foreach (var suite in childSuites)
            {
                all.Add(suite);
                await GetAllSuitesRecursive(client, planId, suite.Id, all);
            }
        }


    }
}
