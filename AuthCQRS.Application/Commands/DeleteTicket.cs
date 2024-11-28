using AuthCQRS.Application.Common.Interfaces;
using MediatR;

namespace AuthCQRS.Application.Commands;
public record DeleteTicketCommand (Guid Id) : IRequest<bool>;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTicketCommandHandler (IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle (DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FindAsync(request.Id);

        if (ticket == null)
        {
            return false;
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
