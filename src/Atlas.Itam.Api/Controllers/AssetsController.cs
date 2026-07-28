using Atlas.Itam.Application.Commands.Assets.CreateAsset;
using Atlas.Itam.Application.Commands.Assets.DeleteAsset;
using Atlas.Itam.Application.Commands.Assets.ReturnFromMaintenance;
using Atlas.Itam.Application.Commands.Assets.RetireAsset;
using Atlas.Itam.Application.Commands.Assets.SendToMaintenance;
using Atlas.Itam.Application.Commands.Assets.TransferAsset;
using Atlas.Itam.Application.Commands.Assets.UpdateAsset;
using Atlas.Itam.Application.Queries.Assets.GetAssetById;
using Atlas.Itam.Application.Queries.Assets.GetAssetMovements;
using Atlas.Itam.Application.Queries.Assets.GetAssets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Itam.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Domain.Enums.AssetStatus? status,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAssetsQuery(search, status, categoryId, locationId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetAssetByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("{id:guid}/movements")]
    public async Task<IActionResult> GetMovements(Guid id)
    {
        var result = await _mediator.Send(new GetAssetMovementsQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> Create([FromBody] CreateAssetCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.AssetId }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetCommand command)
    {
        if (id != command.AssetId)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAssetCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/transfer")]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> Transfer(Guid id, [FromBody] TransferAssetCommand command)
    {
        if (id != command.AssetId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ResponsibleId = userId ?? command.ResponsibleId };
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpPut("{id:guid}/maintenance")]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> SendToMaintenance(Guid id, [FromBody] SendToMaintenanceCommand command)
    {
        if (id != command.AssetId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ResponsibleId = userId ?? command.ResponsibleId };
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpPut("{id:guid}/return-from-maintenance")]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> ReturnFromMaintenance(Guid id, [FromBody] ReturnFromMaintenanceCommand command)
    {
        if (id != command.AssetId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ResponsibleId = userId ?? command.ResponsibleId };
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpPut("{id:guid}/retire")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Retire(Guid id, [FromBody] RetireAssetCommand command)
    {
        if (id != command.AssetId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ResponsibleId = userId ?? command.ResponsibleId };
        await _mediator.Send(cmd);
        return NoContent();
    }
}
