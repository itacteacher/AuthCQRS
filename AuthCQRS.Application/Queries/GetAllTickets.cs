using AuthCQRS.Application.Common.Interfaces;
using AuthCQRS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthCQRS.Application.Queries;
public record GetAllTicketsQuery () : IRequest<List<Ticket>>;

public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, List<Ticket>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTicketsQueryHandler (IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ticket>> Handle (GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tickets.ToListAsync(cancellationToken);
    }
}
