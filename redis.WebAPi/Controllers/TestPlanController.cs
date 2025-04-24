using Microsoft.AspNetCore.Mvc;
using redis.WebAPi.Service;

[ApiController]
[Route("[controller]")]
public class TestPlanController : ControllerBase
{
    private readonly TestPlanDiff _azureService;
    private readonly SnapshotService _snapshotService;

    public TestPlanController(TestPlanDiff azureService, SnapshotService snapshotService)
    {
        _azureService = azureService;
        _snapshotService = snapshotService;
    }

    [HttpPost("snapshot")]
    public async Task<IActionResult> SaveSnapshot(string pat,string planName, string operatorName, string comment)
    {
        var testCases = await _azureService.GetAllTestCasesAsync(pat,planName);
        await _snapshotService.SaveSnapshotAsync(planName, testCases, operatorName, comment);
        return Ok("快照已保存");
    }

    [HttpGet("diff")]
    public async Task<IActionResult> Diff(string pat, string planName)
    {
        var current = await _azureService.GetAllTestCasesAsync(pat,planName);
        var previous = await _snapshotService.GetLatestSnapshotAsync(planName);

        if (previous == null)
            return NotFound("没有找到历史快照");

        var diff = _snapshotService.Diff(previous, current);
        return Ok(diff);
    }
}