namespace redis.WebAPi.Model.TestCase
{
    public class SnapshotEntity
    {
        public int Id { get; set; }
        public string PlanName { get; set; }
        public string Operator { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<TestCaseEntity> TestCases { get; set; }
    }
}
