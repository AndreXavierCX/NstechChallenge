using Nstech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Nstech.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
