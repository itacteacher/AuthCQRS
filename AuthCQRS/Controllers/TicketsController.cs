using AuthCQRS.Application.Commands;
using AuthCQRS.Application.Common.Attributes;
using AuthCQRS.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthCQRS.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController (IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create (CreateTicketCommand command)
    {
        var ticketId = await _mediator.Send(command);

        return Ok(new { Id = ticketId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll ()
    {
        var tickets = await _mediator.Send(new GetAllTicketsQuery());

        return Ok(tickets);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete (Guid id)
    {
        var result = await _mediator.Send(new DeleteTicketCommand(id));

        return result ? NoContent() : NotFound();
    }
}
