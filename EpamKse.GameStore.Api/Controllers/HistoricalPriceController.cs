using EpamKse.GameStore.Services.Services.HistoricalPrice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Authorize(Policy = "UserPolicy")]
[Route("historical-price")]
public class HistoricalPriceController : ControllerBase
{
    private readonly IHistoricalPriceService _historicalPriceService;

    public HistoricalPriceController(IHistoricalPriceService historicalPriceService)
    {
        _historicalPriceService = historicalPriceService;
    }

    [HttpGet("{id:int}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetPricesForGame(int id, int? page = null, int? limit = null)
    {
        var prices = await _historicalPriceService.GetPricesForGame(id, page, limit);
        return Ok(prices);
    }
}