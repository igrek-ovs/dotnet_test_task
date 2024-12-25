using Microsoft.AspNetCore.Mvc;
using TestTask.Services;

namespace TestTask.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MarketController : ControllerBase
{
    private readonly MarketService _marketService;

    public MarketController(MarketService marketService)
    {
        _marketService = marketService;
    }

    [HttpPost]
    public async Task BuyAsync(int userId, int itemId)
    {
        await _marketService.BuyAsync(userId, itemId);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPopularItemsReport()
    {
        var report = await _marketService.GetPopularItemsReportAsync();
        return Ok(report);
    }
}