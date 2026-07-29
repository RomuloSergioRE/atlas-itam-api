using Atlas.Itam.Application.Commands.Requests.ApproveRequest;
using Atlas.Itam.Application.Commands.Requests.CreateRequest;
using Atlas.Itam.Application.Commands.Requests.DeliverRequest;
using Atlas.Itam.Application.Commands.Requests.RejectRequest;
using Atlas.Itam.Application.Commands.Requests.ReturnRequest;
using Atlas.Itam.Application.Queries.Requests.GetDeliveryTerm;
using Atlas.Itam.Application.Queries.Requests.GetRequestById;
using Atlas.Itam.Application.Queries.Requests.GetRequests;
using Atlas.Itam.Application.Queries.Requests.ListPendingApprovals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Itam.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? userId)
    {
        var result = await _mediator.Send(new GetRequestsQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRequestByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,ITManager,Manager")]
    public async Task<IActionResult> GetPendingApprovals([FromQuery] Guid departmentId)
    {
        var result = await _mediator.Send(new ListPendingApprovalsQuery(departmentId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequestCommand command)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { RequestedById = userId ?? command.RequestedById };
        var result = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id = result.RequestId }, result);
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "Admin,ITManager,Manager")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null) return Unauthorized();

        await _mediator.Send(new ApproveRequestCommand(id, userId.Value));
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "Admin,ITManager,Manager")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequestCommand command)
    {
        if (id != command.RequestId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ApprovedById = userId ?? command.ApprovedById };
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpPut("{id:guid}/deliver")]
    [Authorize(Roles = "Admin,ITManager")]
    public async Task<IActionResult> Deliver(Guid id)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId is null) return Unauthorized();

        await _mediator.Send(new DeliverRequestCommand(id, userId.Value));
        return NoContent();
    }

    [HttpGet("{id:guid}/delivery-term")]
    public async Task<IActionResult> GetDeliveryTerm(Guid id)
    {
        var pdf = await _mediator.Send(new GetDeliveryTermQuery(id));
        return File(pdf, "application/pdf", $"delivery-term-{id}.pdf");
    }

    [HttpPut("{id:guid}/return")]
    [Authorize]
    public async Task<IActionResult> Return(Guid id, [FromBody] ReturnRequestCommand command)
    {
        if (id != command.RequestId)
            return BadRequest("ID mismatch");

        var userId = HttpContext.Items["UserId"] as Guid?;
        var cmd = command with { ReturnedById = userId ?? command.ReturnedById };
        await _mediator.Send(cmd);
        return NoContent();
    }
}
