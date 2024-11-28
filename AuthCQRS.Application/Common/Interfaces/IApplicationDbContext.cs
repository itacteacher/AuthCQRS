using AuthCQRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthCQRS.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Ticket> Tickets { get; }
    Task<int> SaveChangesAsync (CancellationToken cancellationToken);
}
