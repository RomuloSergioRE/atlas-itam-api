using Atlas.Itam.Application.Queries.Assets.GetStockSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Itam.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _mediator.Send(new GetStockSummaryQuery());
        return Ok(result);
    }
}
