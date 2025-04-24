namespace redis.WebAPi.Model.TestCase
{
    public class TestCaseEntity
    {
        public int Id { get; set; } // 自增主键
        public int TestCaseId { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public int SuiteId { get; set; }
        public string SuiteName { get; set; }
        public string Steps { get; set; }

        public int SnapshotEntityId { get; set; }
        public SnapshotEntity Snapshot { get; set; }
    }
}
