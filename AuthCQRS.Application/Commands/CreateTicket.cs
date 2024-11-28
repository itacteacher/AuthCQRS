using AuthCQRS.Application.Common.Interfaces;
using AuthCQRS.Domain.Entities;
using MediatR;

namespace AuthCQRS.Application.Commands;
public record CreateTicketCommand (string Title, string Description) : IRequest<Guid>;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTicketCommandHandler (IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle (CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            isActive = true,
            CreatedDate = DateTime.Now
        };

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
